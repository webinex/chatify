import { Button, Form, Input, Select } from 'antd';
import { useAccounts, useAddChat } from '../core';
import { useCallback, useMemo } from 'react';
import { uniqId } from '../util';
import { useLocalizer } from './localizer';

type FormValue = {
  name: string;
  members: string[];
};

export function AddChat() {
  const [add, { isFetching: adding }] = useAddChat();
  const { data: accounts, isFetching } = useAccounts();
  const localizer = useLocalizer();
  const options = useMemo(() => accounts?.map((x) => ({ value: x.id, label: x.name })), [accounts]);

  const onSubmit = useCallback(
    (value: FormValue) =>
      add({
        name: value.name,
        members: value.members,
        requestId: uniqId(),
      }),
    [add],
  );

  return (
    <div className="wxchtf-add-chat">
      <h3>{localizer.addChat.title()}</h3>
      <Form onFinish={onSubmit} layout="vertical" disabled={adding}>
        <Form.Item
          name="name"
          label={localizer.addChat.name.label()}
          rules={[
            { required: true, message: localizer.addChat.name.required() },
            { max: 250, message: localizer.addChat.name.max250() },
          ]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          name="members"
          label={localizer.addChat.members.label()}
          rules={[{ required: true, message: localizer.addChat.members.required() }]}
        >
          <Select mode="multiple" loading={isFetching} options={options} />
        </Form.Item>
        <Button loading={adding} type="primary" block htmlType="submit">
          {localizer.addChat.submitBtn()}
        </Button>
      </Form>
    </div>
  );
}
