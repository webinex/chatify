import { Button, Col, List, Row, Space, Tooltip } from 'antd';
import { chatifyApi } from '../../../core';
import { customize } from '../../customize';
import { useCallback, useMemo } from 'react';
import { Icon } from '../../common/Icon';
import { Avatar } from '../../common/Avatar';
import { useChatContext } from '../ChatContext';
import { LeftOutlined } from '@ant-design/icons';
import { useLocalizer } from '@/ui/localizer';

interface ChatMemberListItemProps {
  id: string;
  chatId: string;
}

function useAdd(chatId: string, accountId: string, withHistory: boolean) {
  const [add, state] = chatifyApi.useAddChatMemberMutation();
  const onAdd = useCallback(
    () => add({ chatId, accountId, withHistory }),
    [add, chatId, accountId, withHistory],
  );
  return [onAdd, state] as const;
}

function useRemove(chatId: string, accountId: string, deleteHistory: boolean) {
  const [remove, state] = chatifyApi.useRemoveChatMemberMutation();
  const onRemove = useCallback(
    () => remove({ chatId, accountId, deleteHistory }),
    [remove, chatId, accountId, deleteHistory],
  );
  return [onRemove, state] as const;
}

const ChatMemberListItem = customize('ChatMemberListItem', (props: ChatMemberListItemProps) => {
  const { chatId } = props;
  const { data: chat } = chatifyApi.useGetChatQuery({ id: chatId });
  const { data: accounts } = chatifyApi.useGetAccountListQuery({});
  const account = accounts!.find((x) => x.id === props.id)!;
  const isMember = chat!.members.some((x) => x.id === account.id);
  const [onAddWithoutHistory] = useAdd(chatId, account.id, false);
  const [onAddWithHistory] = useAdd(chatId, account.id, true);
  const [onRemoveKeepHistory] = useRemove(chatId, account.id, false);
  const [onRemoveDeleteHistory] = useRemove(chatId, account.id, true);

  return (
    <List.Item
      className="wxchtf-chat-members-list-item"
      actions={
        isMember
          ? [
              <Tooltip key="remove" title="Remove (keep history)">
                <Button onClick={onRemoveKeepHistory} type="link" icon={<Icon type="remove-member" />} />
              </Tooltip>,
              <Tooltip key="remove_delete_history" title="Remove (delete history)">
                <Button
                  onClick={onRemoveDeleteHistory}
                  type="link"
                  icon={<Icon type="remove-member-delete-history" />}
                />
              </Tooltip>,
            ]
          : [
              <Tooltip key="add_no_history" title="Add (no history)">
                <Button onClick={onAddWithoutHistory} type="link" icon={<Icon type="add-member" />} />
              </Tooltip>,
              <Tooltip key="add" title="Add (share history)">
                <Button
                  onClick={onAddWithHistory}
                  type="link"
                  icon={<Icon type="add-member-with-history" />}
                />
              </Tooltip>,
            ]
      }
    >
      <Space>
        <Avatar account={account} />
        <span>{account.name}</span>
      </Space>
    </List.Item>
  );
});

export const ChatMembersPanel = customize('ChatMembersPanel', () => {
  const { id, compact, onShowMembers } = useChatContext();
  const localizer = useLocalizer();
  const { data: chat } = chatifyApi.useGetChatQuery({ id });
  const { data: accounts } = chatifyApi.useGetAccountListQuery({});

  const ordered = useMemo(
    () =>
      accounts
        ?.slice()
        .sort(
          (a, b) =>
            (chat?.members.some((m) => m.id === a.id) ? -100 : 0) -
            (chat?.members.some((m) => m.id === b.id) ? -100 : 0) +
            a.name.localeCompare(b.name),
        ),
    [accounts, chat],
  );

  if (!chat?.members || !accounts) {
    return null;
  }

  return (
    <div className="wxchtf-chat-members-panel">
      {compact && (
        <div className="wxchtf-chat-members-panel-header">
          <Row wrap={false} align="middle" gutter={10}>
            <Col flex="none">
              <Button type="text" icon={<LeftOutlined />} onClick={() => onShowMembers(false)} />
            </Col>
            <Col>
              <span className="wxchtf-chat-members-panel-title">{localizer.members.title(chat.name)}</span>
            </Col>
          </Row>
        </div>
      )}
      <List
        className="wxchtf-chat-members-list"
        dataSource={ordered}
        renderItem={(item) => <ChatMemberListItem id={item.id} chatId={id} />}
      />
    </div>
  );
});
