import { useMemo } from 'react';
import { chatifyApi } from '../../../core';
import { customize } from '../../customize';
import { Checkbox, Form, List } from 'antd';
import { useLocalizer } from '../../localizer';
import { Avatar } from '../../common/Avatar';

export interface CreateChatMemberInputProps {
  name: string;
}

interface CheckboxListProps {
  value?: string[];
  onChange?: (value: string[]) => void;
}

function CheckboxList(props: CheckboxListProps) {
  const { value, onChange } = props;
  const { data: accounts, isFetching } = chatifyApi.useGetAccountListQuery();
  const me = chatifyApi.settings.me();
  const dataSource = useMemo(() => accounts?.filter((x) => x.id !== me) ?? [], [accounts, me]);

  return (
    <List
      rowKey="id"
      className="wxchtf-create-chat-member-input-list"
      loading={isFetching}
      dataSource={dataSource}
      renderItem={(x) => {
        const checked = value!.includes(x.id);

        return (
          <List.Item className={checked ? '--selected' : ''}>
            <Checkbox
              checked={checked}
              onChange={() =>
                onChange!(value!.includes(x.id) ? value!.filter((y) => y !== x.id) : [...value!, x.id])
              }
            >
              <List.Item.Meta avatar={<Avatar account={x} />} title={x.name} />
            </Checkbox>
          </List.Item>
        );
      }}
    />
  );
}

export const CreateChatMemberInput = customize(
  'CreateChatMemberInput',
  (props: CreateChatMemberInputProps) => {
    const { name } = props;
    const localizer = useLocalizer();

    return (
      <Form.Item
        label={localizer.addChat.members.label()}
        rules={[{ required: true, message: localizer.addChat.members.required() }]}
        name={name}
      >
        <CheckboxList />
      </Form.Item>
    );
  },
);
