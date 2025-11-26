import { Button, Form } from 'antd';
import { Dayjs } from 'dayjs';
import { customize } from '../../customize';
import { useLocalizer } from '../../localizer';
import { AutoReplyPeriodInput } from './AutoReplyPeriodInput';
import { AutoReplyTextInput } from './AutoReplyTextInput';
import { useEffect } from 'react';

export interface AutoReplyFormValue {
  period: [Dayjs, Dayjs];
  text: string;
}

export interface AutoReplyFormProps {
  value: AutoReplyFormValue;
  onSubmit: (value: AutoReplyFormValue) => void;
  busy?: boolean;
}

export const AUTO_REPLY_FORM_INITIAL_VALUE: AutoReplyFormValue = {
  period: null!,
  text: '',
};

export const AutoReplyForm = customize('AutoReplyForm', (props: AutoReplyFormProps) => {
  const { value, busy = false, onSubmit } = props;
  const localizer = useLocalizer();

  const [form] = Form.useForm<AutoReplyFormValue>();

  useEffect(() => {
    form.setFieldsValue(value);
  }, [value, form]);

  return (
    <Form<AutoReplyFormValue>
      form={form}
      layout="vertical"
      onFinish={onSubmit}
      disabled={busy}
      initialValues={value}
    >
      <AutoReplyPeriodInput name="period" />
      <AutoReplyTextInput name="text" />
      <Button loading={busy} type="primary" block htmlType="submit">
        {localizer.autoReply.submitBtn()}
      </Button>
    </Form>
  );
});
