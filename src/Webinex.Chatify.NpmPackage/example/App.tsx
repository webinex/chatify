import * as React from 'react';
import './App.css';
import '../dist/css/chatify.css';
import { ChatifyClient, chatifyApi } from '../src';
import { combineReducers, configureStore } from '@reduxjs/toolkit';
import { Provider } from 'react-redux';
import { LoginPage } from './pages/LoginPage';
import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom';
import { ChatGroupPage } from './pages/ChatGroupPage';
import { Layout } from './pages/Layout';
import { CreateThreadPage } from './pages/CreateThreadPage';
import { ThreadPage } from './pages/ThreadPage';
import { WatchThreadListPage } from './pages/WatchThreadListPage';
import { ChatPage } from './pages/ChatPage';
import { CHATIFY_AXIOS } from './client';

function useReduxStore(me: string | null) {
  return React.useMemo(
    () =>
      me
        ? configureStore({
            reducer: combineReducers({
              [chatifyApi.reducerPath]: chatifyApi.reducer,
            }),
            middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(chatifyApi.middleware),
          })
        : null,
    [me],
  );
}

function useMe() {
  const [me, setMe] = React.useState(() => localStorage.getItem('chatify://me') ?? null);
  React.useEffect(
    () => (me ? localStorage.setItem('chatify://me', me) : localStorage.removeItem('chatify://me')),
    [me],
  );
  return [me, setMe] as const;
}

function useConfigureChatify(me: string | null) {
  React.useMemo(() => {
    if (!me) {
      return null;
    }

    const chatifyClient = new ChatifyClient({
      axios: CHATIFY_AXIOS,
      signalR: {
        hubUri: process.env['HUB_URI'],
        headersFactory: () => Promise.resolve({ 'X-USER-ID': localStorage.getItem('chatify://me')! }),
        accessTokenFactory: () => Promise.resolve(me),
      },
    });

    chatifyApi.settings.client = chatifyClient;
    chatifyApi.settings.me = () => me;
  }, [me]);
}

export function App() {
  const [me, setMe] = useMe();
  useConfigureChatify(me);
  const store = useReduxStore(me);
  const modal = window.location.search.includes('modal');

  if (!me) {
    return <LoginPage onLoggedIn={setMe} />;
  }

  return (
    <Provider store={store!}>
      <BrowserRouter>
        {!modal && (
          <Layout me={me} onLogout={() => setMe(null)}>
            <Routes>
              <Route path="/chat" element={<ChatGroupPage />} />
              <Route path="/chat/:id" element={<ChatPage />} />
              <Route path="/thread/watch" element={<WatchThreadListPage />} />
              <Route path="/thread/add" element={<CreateThreadPage me={me} />} />
              <Route path="/thread/:id" element={<ThreadPage />} />
              <Route index element={<Navigate to="/chat" />} />
            </Routes>
          </Layout>
        )}
        {modal && (
          <Routes>
            <Route path="/chat/:id" element={<ChatPage modal />} />
          </Routes>
        )}
      </BrowserRouter>
    </Provider>
  );
}
