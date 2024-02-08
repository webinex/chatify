import { Form, Input } from 'antd';
import { customize } from '../customize';
import { useLocalizer } from '../localizer';

export interface CreateChatNameInputProps {
  name: string;
}

export const CreateChatNameInput = customize('CreateChatNameInput', (props: CreateChatNameInputProps) => {
  const { name } = props;
  const localizer = useLocalizer();

  return (
    <Form.Item
      name={name}
      label={localizer.addChat.name.label()}
      rules={[
        { required: true, message: localizer.addChat.name.required() },
        { max: 250, message: localizer.addChat.name.max250() },
      ]}
    >
      <Input />
    </Form.Item>
  );
});
