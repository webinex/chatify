import React, { useCallback } from 'react';
import { Avatar, ThreadWatchListItem, calcThreadUnreadCount, chatifyApi } from '../../src';
import { Button, Card, Col, Flex, Row, Tag, Typography } from 'antd';
import { useNavigate } from 'react-router-dom';
import { EyeInvisibleOutlined } from '@ant-design/icons';

function Item({ thread }: { thread: ThreadWatchListItem }) {
  const unreadCount = calcThreadUnreadCount(thread);
  const navigate = useNavigate();
  const [watchThread, { isLoading: isWatchFetching }] = chatifyApi.useWatchThreadMutation();
  const onUnwatch = useCallback(
    (e: React.MouseEvent<HTMLButtonElement>) => {
      e.stopPropagation();
      watchThread({ id: thread.id, watch: false }).unwrap();
    },
    [watchThread, thread.id],
  );

  return (
    <Card
      style={{ height: '100%' }}
      className="clickable"
      title={
        <Flex justify="space-between">
          <Typography.Text style={{ fontWeight: unreadCount > 0 ? 600 : 400 }}>{thread.name}</Typography.Text>
          <span>
            {unreadCount > 0 && <Tag color="orange">{unreadCount}</Tag>}
            <Button
              onClick={onUnwatch}
              loading={isWatchFetching}
              type="link"
              icon={<EyeInvisibleOutlined />}
            />
          </span>
        </Flex>
      }
      onClick={() => navigate(`/thread/${thread.id}`)}
    >
      {thread.lastMessage && (
        <Flex gap={10}>
          <div style={{ flex: 'none' }}>
            <Avatar account={thread.lastMessage.sentBy} />
          </div>
          <div style={{ minWidth: 1 }}>
            <div style={{ fontWeight: 500 }}>{thread.lastMessage.sentBy.name}</div>
            <div>
              <Typography.Text ellipsis type="secondary">
                {thread.lastMessage.text}
              </Typography.Text>
            </div>
          </div>
        </Flex>
      )}
      {!thread.lastMessage && (
        <Typography.Text italic type="secondary">
          No Messages
        </Typography.Text>
      )}
    </Card>
  );
}

export function WatchThreadListPage() {
  const { data: threads } = chatifyApi.useGetWatchThreadListQuery();

  if (threads == null) {
    return null;
  }

  return (
    <div>
      <Row gutter={[20, 20]}>
        {threads.map((thread) => (
          <Col span={8} key={thread.id}>
            <Item thread={thread} />
          </Col>
        ))}
      </Row>
    </div>
  );
}
