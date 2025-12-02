import React, { PropsWithChildren } from 'react';
import { Avatar, Button } from 'antd';
import { Account, chatifyApi, useUnreadThreadMessageCount } from '../../src';
import { NavLink } from 'react-router-dom';

export interface LayoutProps extends PropsWithChildren<{}> {
  me: string | null;
  onLogout: () => void;
}

function useUnreadChatMessageCount() {
  const { data: chats } = chatifyApi.useGetChatListQuery();
  let count = 0;
  chats?.forEach((x) => (count += x.totalUnreadCount));
  return chats ? count : undefined;
}

export function Layout(props: LayoutProps) {
  const { children, me, onLogout } = props;
  const [account, setAccount] = React.useState<Account | null>(null);
  const unreadChatMessageCount = useUnreadChatMessageCount();
  const [unreadThreadMessageCount] = useUnreadThreadMessageCount();

  React.useEffect(() => {
    if (me) {
      fetch('/api/account/' + me)
        .then((x) => x.json())
        .then((x) => setAccount(x));
    } else {
      setAccount(null);
    }
  }, [me]);

  return (
    <div className="layout">
      {account && (
        <header>
          <div className="user-info">
            {account && (
              <>
                <Avatar src={account.avatar} className="avatar" />{' '}
                <span className="name">
                  {' '}
                  {account.name} (ID: {account.id})
                </span>
              </>
            )}
          </div>
          <div className="nav">
            <NavLink to="/audit">Audit</NavLink>
            <NavLink to="/thread/add">+ Add Thread</NavLink>
            <NavLink to="/thread/watch">Threads ({unreadThreadMessageCount ?? 0})</NavLink>
            <NavLink to="/chat">Chat ({unreadChatMessageCount})</NavLink>
          </div>
          <Button type="text" className="btn-logout" onClick={onLogout}>
            Logout
          </Button>
        </header>
      )}

      <main>{children}</main>
    </div>
  );
}
