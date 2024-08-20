import { BaseQueryFn, EndpointBuilder, createApi, fakeBaseQuery } from '@reduxjs/toolkit/query/react';
import { ChatifyClient } from '../client';
import { Debounce } from '../debounce';
import { chatId } from '../types';

export const DEFAULT_MESSAGE_LIST_TAKE = 20;

export const CHAT_MESSAGE_READ_QUEUE = new Debounce<string>({
  callback: (id) => __settings.client.readChatMessage({ id }),
  compare: (a, b) => -a.localeCompare(b),
  groupBy: (id) => chatId(id),
  timeout: 500,
});

export interface ChatifyApiSettings {
  me: () => string;
  client: ChatifyClient;
}

export const __settings: ChatifyApiSettings = {
  me: null!,
  client: null!,
};

const TAG_TYPES = ['chat', 'account', 'message', 'thread', 'thread-message'] as const;

export interface BaseApiEndpointBuilder
  extends EndpointBuilder<BaseQueryFn, (typeof TAG_TYPES)[number], 'chatify'> {}

const baseApi: ReturnType<typeof createApi<BaseQueryFn, {}, 'chatify', (typeof TAG_TYPES)[number]>> =
  createApi({
    baseQuery: fakeBaseQuery(),
    reducerPath: 'chatify',
    tagTypes: TAG_TYPES,
    endpoints: (_) => ({}),
  });

export const __baseApi = baseApi;
