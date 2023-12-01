import { AxiosInstance } from 'axios';
import * as SignalR from '@microsoft/signalr';
import {
  Account,
  AddChatRequest,
  AddMemberRequest,
  Chat,
  ChatListItem,
  Message,
  ReadRequest,
  RemoveMemberRequest,
  SendMessageRequest,
} from './models';

export interface ChatifySignalRConfig {
  hubUri?: string;
  accessTokenFactory?: () => Promise<string>;
  headersFactory?: () => Promise<Record<string, string>>;
}

const DEFAULT_SIGNALR_CONFIG: ChatifySignalRConfig = {
  hubUri: '/api/chatify/hub',
  headersFactory: () => Promise.resolve({}),
};

export interface ChatifyClientConfig {
  axios: AxiosInstance;
  signalR: ChatifySignalRConfig;
}

const METHODS = [
  'chatify://new-message',
  'chatify://chat-created',
  'chatify://read',
  'chatify://member-added',
  'chatify://member-removed',
] as const;

export class ChatifyClient {
  private _config: ChatifyClientConfig;
  private _connection: SignalR.HubConnection = null!;
  private _subscribers: Record<string, ((args: any[]) => void)[]> = {};
  private _reconnectSubscribers: Array<() => void> = [];

  constructor(config: ChatifyClientConfig) {
    this._config = {
      ...config,
      signalR: {
        ...DEFAULT_SIGNALR_CONFIG,
        ...config.signalR,
      },
    };
  }

  public connect = async () => {
    const headers = await this._config.signalR!.headersFactory!();

    this._connection = new SignalR.HubConnectionBuilder()
      .withUrl(this._config.signalR!.hubUri!, {
        accessTokenFactory: this._config.signalR!.accessTokenFactory
          ? this._config.signalR!.accessTokenFactory
          : undefined,
        headers,
      })
      .withAutomaticReconnect()
      .build();

    METHODS.forEach((x) => {
      this._connection.on(x, (...args) => {
        this._subscribers[x]?.forEach((s) => s(args));
      });
    });

    this._connection.onreconnected(() => this._reconnectSubscribers.forEach((subscriber) => subscriber()));

    await this._connection.start();
  };

  public subscribe = <T extends any[]>(method: string, subscriber: (args: T) => void) => {
    this._subscribers[method] = this._subscribers[method] ?? [];
    this._subscribers[method].push(subscriber as any);
    return () => this._subscribers[method].filter((x) => x !== subscriber);
  };

  public subscribeReconnect = (subscriber: () => any) => {
    this._reconnectSubscribers.push(subscriber);
    return () => this._reconnectSubscribers.filter((x) => x !== subscriber);
  };

  private get axios() {
    return this._config.axios;
  }

  public accounts = async () => {
    const { data } = await this.axios.get<Account[]>('account');
    return data;
  };

  public chats = async () => {
    const { data } = await this.axios.get<ChatListItem[]>('chat');
    return data;
  };

  public chat = async (id: string) => {
    const { data } = await this.axios.get<Chat>(`chat/${id}`);
    return data;
  };

  public addChat = async (args: AddChatRequest) => {
    const { data } = await this.axios.post<string>(`chat`, args);
    return data;
  };

  public messages = async (args: { chatId: string; skip?: number; take?: number }) => {
    const { chatId, skip = 0, take = 20 } = args;
    const pagingRule = encodeURIComponent(JSON.stringify({ skip, take }));
    const { data } = await this.axios.get<Message[]>(`chat/${chatId}/message?pagingRule=${pagingRule}`);
    return data;
  };

  public send = async (request: SendMessageRequest) => {
    const { chatId, ...rest } = request;
    await this.axios.post(`chat/${chatId}/message`, rest);
  };

  public read = async (request: ReadRequest) => {
    await this.axios.put(`chat/message/read`, request);
  };

  public removeMember = async (request: RemoveMemberRequest) => {
    await this.axios.delete(`chat/${request.chatId}/member`, { data: request });
  };

  public addMember = async (request: AddMemberRequest) => {
    await this.axios.post(`chat/${request.chatId}/member`, request);
  };
}
