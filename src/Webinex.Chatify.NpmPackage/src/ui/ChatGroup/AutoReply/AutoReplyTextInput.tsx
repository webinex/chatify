import { Form, Input } from 'antd';
import { customize } from '../../customize';
import { useLocalizer } from '../../localizer';

export interface AutoReplyTextInputProps {
  name: string;
}

export const AutoReplyTextInput = customize('AutoReplyTextInput', (props: AutoReplyTextInputProps) => {
  const { name } = props;
  const localizer = useLocalizer();

  return (
    <Form.Item
      label={localizer.autoReply.text.label()}
      name={name}
      rules={[{ required: true, message: localizer.autoReply.text.required() }]}
    >
      <Input.TextArea rows={4} />
    </Form.Item>
  );
});
