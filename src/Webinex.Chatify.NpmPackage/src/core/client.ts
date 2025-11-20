import { AxiosInstance } from 'axios';
import * as SignalR from '@microsoft/signalr';
import {
  Account,
  AddChatRequest,
  AddChatMemberRequest,
  Chat,
  ChatListItem,
  ChatMessage,
  ReadChatMessageEvent,
  ReadChatMessageRequest,
  RemoveChatMemberRequest,
  SendChatMessageRequest,
  UpdateChatNameRequest,
  SendThreadMessageRequest,
  Thread,
  ThreadMessage,
  ThreadWatchListItem as ThreadListItem,
  UpdateAccountRequest,
} from './types';

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

export const CHATIFY_SIGNALR_METHODS = [
  'chatify://chat-new-message',
  'chatify://chat-created',
  'chatify://chat-message-read',
  'chatify://chat-member-added',
  'chatify://chat-member-removed',
  'chatify://chat-name-changed',

  'chatify://thread-created',
  'chatify://thread-message-new',
  'chatify://thread-message-read',
  'chatify://thread-watch-added',
  'chatify://thread-watch-removed',
  'chatify://thread-updated',
] as const;

export type ChatifySignalRMethod = (typeof CHATIFY_SIGNALR_METHODS)[number];

export type ChatifySignalRArgsByMethod = {
  'chatify://chat-new-message': [message: ChatMessage];
  'chatify://chat-created': [chat: ChatListItem];
  'chatify://chat-message-read': [events: ReadChatMessageEvent[]];
  'chatify://chat-member-added': [
    chatId: string,
    chatName: string,
    account: Account,
    message: ChatMessage,
    withHistory: boolean,
    read: boolean,
  ];
  'chatify://chat-member-removed': [
    chatId: string,
    accountId: string,
    deleteHistory: boolean,
    message: ChatMessage,
    read: boolean,
  ];
  'chatify://chat-name-changed': [chatId: string, newName: string, message: ChatMessage, read: boolean];

  'chatify://thread-message-new': [threadId: string, message: ThreadMessage, readForId: string | null];
  'chatify://thread-message-read': [threadId: string, messageId: string];
  'chatify://thread-watch-added': [thread: ThreadListItem];
  'chatify://thread-watch-removed': [id: string];
  'chatify://thread-created': [thread: ThreadListItem, watch: boolean];
  'chatify://thread-updated': [threadId: string, threadName: string];
};

export class ChatifyClient {
  private _config: ChatifyClientConfig;
  private _connection: SignalR.HubConnection = null!;
  private _connectionPromise: Promise<SignalR.HubConnection> | null = null!;
  private _subscribers: Record<string, ((args: any[]) => void)[]> = {};
  private _reconnectSubscribers: Array<() => void> = [];
  private _threadConnectionByThreadId = new Map<string, number>();

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
    if (this._connection) {
      return;
    }

    if (this._connectionPromise) {
      return await this._connectionPromise;
    }

    this._connectionPromise = this.createAndStartConnection();
    this._connection = await this._connectionPromise;
  };

  private createAndStartConnection = async () => {
    const headers = await this._config.signalR!.headersFactory!();

    const connection = new SignalR.HubConnectionBuilder()
      .withUrl(this._config.signalR!.hubUri!, {
        accessTokenFactory: this._config.signalR!.accessTokenFactory
          ? this._config.signalR!.accessTokenFactory
          : undefined,
        headers,
      })
      .withAutomaticReconnect()
      .build();

    CHATIFY_SIGNALR_METHODS.forEach((x) => {
      connection.on(x, (...args) => {
        this._subscribers[x]?.forEach((s) => {
          try {
            s(args);
          } catch (ex) {
            console.error(`Failed to notify subscriber`, ex);
          }
        });
      });
    });

    connection.onreconnected(() => this._reconnectSubscribers.forEach((subscriber) => subscriber()));

    await connection.start();
    return connection;
  };

  public subscribe = <TMethod extends ChatifySignalRMethod>(
    method: TMethod,
    subscriber: (args: ChatifySignalRArgsByMethod[TMethod]) => void,
  ) => {
    this.connect().then(() => {
      this._subscribers[method] = this._subscribers[method] ?? [];
      this._subscribers[method].push(subscriber as any);
    });
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

  public updateAccount = async (request: UpdateAccountRequest) => {
    await this.axios.put(`account/${encodeURIComponent(request.id)}`, request);
  };

  public chats = async () => {
    const { data } = await this.axios.get<ChatListItem[]>('chat');
    return data;
  };

  public chat = async (id: string) => {
    const { data } = await this.axios.get<Chat>(`chat/${encodeURIComponent(id)}`);
    return data;
  };

  public addChat = async (args: AddChatRequest) => {
    const { data } = await this.axios.post<string>(`chat`, args);
    return data;
  };

  public chatMessages = async (args: { chatId: string; skip?: number; take?: number }) => {
    const { chatId, skip = 0, take = 20 } = args;
    const pagingRule = encodeURIComponent(JSON.stringify({ skip, take }));
    const { data } = await this.axios.get<ChatMessage[]>(
      `chat/${encodeURIComponent(chatId)}/message?pagingRule=${pagingRule}`,
    );
    return data;
  };

  public sendChatMessage = async (request: SendChatMessageRequest) => {
    const { chatId, ...rest } = request;
    await this.axios.post(`chat/${encodeURIComponent(chatId)}/message`, rest);
  };

  public readChatMessage = async (request: ReadChatMessageRequest) => {
    await this.axios.put(`chat/message/read`, request);
  };

  public removeChatMember = async (request: RemoveChatMemberRequest) => {
    await this.axios.delete(`chat/${encodeURIComponent(request.chatId)}/member`, { data: request });
  };

  public updateChatName = async (request: UpdateChatNameRequest) => {
    await this.axios.put(`chat/${encodeURIComponent(request.id)}/name`, request);
  };

  public addChatMember = async (request: AddChatMemberRequest) => {
    await this.axios.post(`chat/${encodeURIComponent(request.chatId)}/member`, request);
  };

  public sendThreadMessage = async ({ body, threadId }: SendThreadMessageRequest) => {
    const { data: id } = await this.axios.post<string>(`thread/${encodeURIComponent(threadId)}/message`, {
      body,
    });
    return id;
  };

  public readThreadMessage = async (id: string) => {
    await this.axios.put(`thread/message/${encodeURIComponent(id)}/read`);
  };

  public watchThreads = async () => {
    const { data } = await this.axios.get<ThreadListItem[]>(`thread/watch`);
    return data;
  };

  public thread = async (id: string) => {
    const { data } = await this.axios.get<Thread>(`thread/${encodeURIComponent(id)}`);
    return data;
  };

  public threadMessages = async ({
    threadId,
    skip,
    take,
  }: {
    threadId: string;
    skip: number;
    take: number;
  }) => {
    const searchParams = new URLSearchParams();
    searchParams.append('skip', skip.toString());
    searchParams.append('take', take.toString());
    const { data } = await this.axios.get<ThreadMessage[]>(
      `thread/${encodeURIComponent(threadId)}/message?${searchParams.toString()}`,
    );
    return data;
  };

  public watchThread = async (id: string, watch: boolean) => {
    await this.axios.put(`thread/${encodeURIComponent(id)}/watch`, { watch });
  };

  public connectToThread = async (id: string) => {
    const connectionCount = this._threadConnectionByThreadId.get(id) ?? 0;
    this._threadConnectionByThreadId.set(id, connectionCount + 1);

    if (connectionCount === 0) {
      this.connect().then(() => this._connection.invoke('ConnectToThread', id));
    }
  };

  public disconnectFromThread = async (id: string) => {
    const connectionCount = this._threadConnectionByThreadId.get(id) ?? 0;
    this._threadConnectionByThreadId.set(id, Math.max(0, connectionCount - 1));

    if (connectionCount === 1) {
      this.connect().then(() => this._connection.invoke('DisconnectFromThread', id));
    }
  };
}

export function unsubscribe(...subscribers: Array<() => void>) {
  const result = () => subscribers.forEach((x) => x());
  result.when = (promise: Promise<any>) => promise.then(result);
  return result;
}
