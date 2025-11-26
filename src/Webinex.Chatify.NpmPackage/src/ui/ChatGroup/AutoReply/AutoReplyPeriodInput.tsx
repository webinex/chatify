import { DatePicker, Form } from 'antd';
import { customize } from '../../customize';
import { useLocalizer } from '../../localizer';

export interface AutoReplyPeriodInputProps {
  name: string;
}

export const AutoReplyPeriodInput = customize('AutoReplyPeriodInput', (props: AutoReplyPeriodInputProps) => {
  const { name } = props;
  const localizer = useLocalizer();

  return (
    <Form.Item
      label={localizer.autoReply.period.label()}
      name={name}
      rules={[{ required: true, message: localizer.autoReply.period.required() }]}
    >
      <DatePicker.RangePicker format={localizer.autoReply.period.format()} />
    </Form.Item>
  );
});
