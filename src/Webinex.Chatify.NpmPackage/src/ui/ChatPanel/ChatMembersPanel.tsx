import { Button, List, Space, Tooltip } from 'antd';
import {
  useAddMemberMutation,
  useGetAccountsQuery,
  useGetChatQuery,
  useRemoveMemberMutation,
} from '../../core';
import { customize } from '../customize';
import { useCallback, useMemo } from 'react';
import { Icon } from '../Icon';
import { Avatar } from '../Avatar';

export interface ChatMembersPanelProps {
  id: string;
}

interface ChatMemberListItemProps {
  id: string;
  chatId: string;
}

function useAdd(chatId: string, accountId: string, withHistory: boolean) {
  const [add, state] = useAddMemberMutation();
  const onAdd = useCallback(
    () => add({ chatId, accountId, withHistory }),
    [add, chatId, accountId, withHistory],
  );
  return [onAdd, state] as const;
}

function useRemove(chatId: string, accountId: string, deleteHistory: boolean) {
  const [remove, state] = useRemoveMemberMutation();
  const onRemove = useCallback(
    () => remove({ chatId, accountId, deleteHistory }),
    [remove, chatId, accountId, deleteHistory],
  );
  return [onRemove, state] as const;
}

const ChatMemberListItem = customize('ChatMemberListItem', (props: ChatMemberListItemProps) => {
  const { chatId } = props;
  const { data: chat } = useGetChatQuery({ chatId });
  const { data: accounts } = useGetAccountsQuery();
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
                <Button onClick={onRemoveKeepHistory} type="link" icon={<Icon type="remove" />} />
              </Tooltip>,
              <Tooltip key="remove_delete_history" title="Remove (delete history)">
                <Button
                  onClick={onRemoveDeleteHistory}
                  type="link"
                  icon={<Icon type="remove_delete_history" />}
                />
              </Tooltip>,
            ]
          : [
              <Tooltip key="add_no_history" title="Add (no history)">
                <Button onClick={onAddWithoutHistory} type="link" icon={<Icon type="add" />} />
              </Tooltip>,
              <Tooltip key="add" title="Add (share history)">
                <Button onClick={onAddWithHistory} type="link" icon={<Icon type="add_with_history" />} />
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

export const ChatMembersPanel = customize('ChatMembersPanel', (props: ChatMembersPanelProps) => {
  const { id } = props;

  const { data: chat } = useGetChatQuery({ chatId: id });
  const { data: accounts } = useGetAccountsQuery();
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
      <List
        className="wxchtf-chat-members-list"
        dataSource={ordered}
        renderItem={(item) => <ChatMemberListItem id={item.id} chatId={id} />}
      />
    </div>
  );
});
