import * as React from 'react';
import './App.css';
import '../dist/css/chatify.css';
import { Chatify, ChatifyClient, ChatifyContext, ChatifyCustomizeValue, useGetChatListQuery } from '../src';
import axios from 'axios';
import { Avatar, Button, Select } from 'antd';
import { Account } from '../src/core/models';

const ME = localStorage.getItem('chatify://me') ?? '1';

export function App() {
  const [accounts, setAccounts] = React.useState<Account[]>([]);

  React.useEffect(() => {
    fetch('/api/account')
      .then((x) => x.json())
      .then((x) => setAccounts(x));
  }, []);

  if (localStorage.getItem('chatify://me') === null) {
    return (
      <div className="example">
        <div className="user-select-box">
          <h3>Please select user</h3>
          <Select
            className="user-select"
            placeholder="Select user..."
            onChange={(value) => {
              localStorage.setItem('chatify://me', value);
              window.location.href = window.location.href;
            }}
            options={accounts.map((x) => ({
              value: x.id,
              label: (
                <>
                  <Avatar src={x.avatar} /> {x.name}
                </>
              ),
            }))}
          />
        </div>
      </div>
    );
  }

  return <Content />;
}

const axiosInstance = axios.create({ baseURL: '/api/chatify' });
axiosInstance.interceptors.request.use((request) => {
  request.headers['X-USER-ID'] = localStorage.getItem('chatify://me');
  return request;
});

const chatifyClient = new ChatifyClient({
  axios: axiosInstance,
  signalR: {
    hubUri: process.env['HUB_URI'],
    headersFactory: () => Promise.resolve({ 'X-USER-ID': localStorage.getItem('chatify://me')! }),
    accessTokenFactory: () => Promise.resolve(ME),
  },
});

const customize: ChatifyCustomizeValue = {};

function useCount() {
  const { data: chats } = useGetChatListQuery();
  let count = 0;
  chats?.forEach((x) => (count += x.totalUnreadCount));
  return chats ? count : undefined;
}

function Counter() {
  const count = useCount();
  return <>Unread: {count}</>;
}

function Content() {
  const [account, setAccount] = React.useState<Account | null>(null);

  React.useEffect(() => {
    fetch('/api/account/' + ME)
      .then((x) => x.json())
      .then((x) => setAccount(x));
  }, []);

  return (
    <ChatifyContext value={{ client: chatifyClient, me: ME }}>
      <div className="example">
        <div className="user-info">
          {account && (
            <>
              <Avatar src={account.avatar} className="avatar" />{' '}
              <span className="name">
                {' '}
                {account.name} (ID: {account.id})
              </span>
              <span className="unread">
                <Counter />
              </span>
            </>
          )}
          <Button
            type="link"
            onClick={() => {
              localStorage.removeItem('chatify://me');
              window.location.href = window.location.href;
            }}
          >
            Logout
          </Button>
        </div>
        <Chatify customize={customize} />
      </div>
    </ChatifyContext>
  );
}
