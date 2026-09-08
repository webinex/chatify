import React, { PropsWithChildren, useEffect, useRef, useState } from 'react';
import { Avatar, Button, Dropdown } from 'antd';
import { Account, chatifyApi, useUnreadThreadMessageCount } from '../../src';
import { NavLink, useNavigate } from 'react-router-dom';
import { MenuOutlined } from '@ant-design/icons';

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

function useCompact(threshold: number = 768) {
  const ref = useRef<HTMLDivElement>(null);
  const [size, setSize] = useState<{ width: number; height: number }>();

  useEffect(() => {
    const el = ref.current;
    if (!el) return;

    const resizeObserver = new ResizeObserver(() => {
      const { width, height } = el.getBoundingClientRect();
      setSize({ width, height });
    });

    resizeObserver.observe(el);

    return () => {
      resizeObserver.disconnect();
    };
  }, []);

  const compact = size != null ? size.width < threshold : undefined;
  return [ref, compact, size] as const;
}

export function Layout(props: LayoutProps) {
  const { children, me, onLogout } = props;
  const [account, setAccount] = React.useState<Account | null>(null);
  const unreadChatMessageCount = useUnreadChatMessageCount();
  const [unreadThreadMessageCount] = useUnreadThreadMessageCount();
  const open = useNavigate();
  const [ref, compact] = useCompact();

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
    <div className="layout" ref={ref}>
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
          {compact === false && (
            <>
              <div className="nav">
                <NavLink to="/audit">Audit</NavLink>
                <NavLink to="/thread/add">+ Add Thread</NavLink>
                <NavLink to="/thread/watch">Threads ({unreadThreadMessageCount ?? 0})</NavLink>
                <NavLink to="/chat">Chat ({unreadChatMessageCount})</NavLink>
              </div>
              <Button type="text" className="btn-logout" onClick={onLogout}>
                Logout
              </Button>
            </>
          )}

          {compact === true && (
            <Dropdown
              menu={{
                items: [
                  { key: '/audit', label: 'Audit', onClick: () => open('/audit') },
                  { key: '/thread/add', label: '+ Add Thread', onClick: () => open('/thread/add') },
                  {
                    key: '/thread/watch',
                    label: `Threads (${unreadThreadMessageCount ?? 0})`,
                    onClick: () => open('/thread/watch'),
                  },
                  { key: '/chat', label: `Chat (${unreadChatMessageCount})`, onClick: () => open('/chat') },
                  { key: '/logout', label: 'Logout', onClick: onLogout },
                ],
              }}
            >
              <Button type="text" style={{ marginInlineStart: 'auto' }}>
                <MenuOutlined />
              </Button>
            </Dropdown>
          )}
        </header>
      )}

      <main>{children}</main>
    </div>
  );
}
