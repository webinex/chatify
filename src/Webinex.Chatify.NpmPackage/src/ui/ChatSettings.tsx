import { Button, Col, List, Row, Select } from 'antd';
import { Account, useAccounts, useAddMember, useChat, useSelect } from '../core';
import { CloseOutlined } from '@ant-design/icons';
import { useRemoveMember } from '../core';
import { useCallback, useMemo } from 'react';

function Member(props: { account: Account; chatId: string }) {
  const { account, chatId } = props;
  const [remove] = useRemoveMember();

  const onRemoveClick = useCallback(
    () => remove({ chatId, accountId: account.id, deleteHistory: false }),
    [remove, chatId, account.id],
  );

  return (
    <List.Item>
      {account.name} <Button onClick={onRemoveClick} type="link" icon={<CloseOutlined />} />
    </List.Item>
  );
}

export function ChatSettings() {
  const [chatId] = useSelect((x) => [x.ui.chatId], []);
  const { data: chat } = useChat({ chatId: chatId! });
  const [add] = useAddMember();
  const { data: accounts } = useAccounts();
  const accountOptions = useMemo(() => accounts?.map((x) => ({ value: x.id, label: x.name })), [accounts]);

  const onAdd = useCallback(
    (id: string) => add({ chatId: chatId!, accountId: id, withHistory: true }),
    [add, chatId],
  );

  if (!chat) {
    return null;
  }

  return (
    <div className="wxchtf-chat-settings">
      <Row justify="space-between">
        <Col>
          <h3 className="wxchtf-title">{chat.name}</h3>
        </Col>
        <Col>
          <Button type="link" icon={<CloseOutlined />} />
        </Col>
      </Row>
      <h4>Members</h4>
      <List
        dataSource={chat.members}
        rowKey="id"
        renderItem={(account) => <Member account={account} chatId={chatId!} />}
      />

      <Select<string> style={{ width: '350px' }} options={accountOptions} value={null} onSelect={onAdd} />
    </div>
  );
}
