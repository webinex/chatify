import { PagingRule } from '@webinex/asky';
import { Account, chatifyApi, File, ListSegment, Optional } from '@webinex/chatify';

export interface GetAuditChatListQueryArgs {
  containsOneOfMembers: string[] | null;
  searchString: string | null;
  pagingRule: PagingRule | null;
  includeTotal: boolean | null;
}

export interface AuditChatResponse {
  id: string;
  name: string;
  createdAt: string;
  createdById: string;
  lastMessage: Optional<AuditChatMessage>;
}

export interface AuditChatMessage {
  id: string;
  authorId: string;
  sentAt: string;
  text: string | null;
  files: File[];
  sentBy: Account;
}

export interface AuditChatListItem {
  id: string;
  name: string;
  createdAt: string;
  createdById: string;
  lastMessage: AuditChatMessage;
}

export interface AuditChat {
  id: string;
  name: string;
  createdAt: string;
  createdById: string;
}

function qs(params: Record<string, any>): string {
  const query = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== null && value !== undefined) {
      query.append(key, typeof value === 'string' ? value : JSON.stringify(value));
    }
  });

  return query.toString();
}

export const auditApi = chatifyApi.injectEndpoints({
  endpoints: (build) => ({
    getAuditChatList: build.query<ListSegment<AuditChatListItem>, GetAuditChatListQueryArgs>({
      queryFn: async (args) =>
        chatifyApi.settings.client.axios
          .get<ListSegment<AuditChatResponse>>(`audit/chats?${qs({ query: args })}`)
          .then(({ data }) => ({
            data: {
              total: data.total,
              items: data.items.map(
                ({ lastMessage, ...x }): AuditChatListItem => ({ ...x, lastMessage: lastMessage.value! }),
              ),
            },
          }))
          .catch((error) => ({ error })),
      providesTags: ['chat', 'message'],
    }),

    getAuditChat: build.query<AuditChat, { id: string }>({
      queryFn: async (args) =>
        chatifyApi.settings.client.axios
          .get(`audit/chats/${encodeURIComponent(args.id)}`)
          .then(({ data }) => ({ data }))
          .catch((error) => ({ error })),
      providesTags: ['chat'],
    }),

    getAuditChatMessagesInfinite: build.infiniteQuery<
      ListSegment<AuditChatMessage>,
      { id: string },
      { skip: number; take: number }
    >({
      queryFn: async (args) => {
        const {
          pageParam,
          queryArg: { id },
        } = args;

        return chatifyApi.settings.client.axios
          .get(`audit/messages?${qs({ query: { chatId: id, pagingRule: pageParam, includeTotal: true } })}`)
          .then(({ data }) => ({ data }))
          .catch((error) => ({ error }));
      },
      infiniteQueryOptions: {
        initialPageParam: { skip: 0, take: 20 },
        getNextPageParam: (lastPage, _, lastPageParam) =>
          lastPage.total > lastPageParam.skip + lastPageParam.take
            ? {
                skip: lastPageParam.skip + lastPageParam.take,
                take: lastPageParam.take,
              }
            : undefined,
      },
      providesTags: ['message', 'chat'],
    }),
  }),
});
