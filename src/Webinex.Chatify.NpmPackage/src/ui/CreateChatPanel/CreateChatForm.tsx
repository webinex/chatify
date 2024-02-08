import { Button, Form } from 'antd';
import { customize } from '../customize';
import { CreateChatNameInput } from './CreateChatNameInput';
import { CreateChatMemberInput } from './CreateChatMemberInput';
import { useLocalizer } from '../localizer';

export interface CreateChatFormValue {
  name: string;
  members: string[];
}

export interface CreateChatFormProps {
  value?: CreateChatFormValue;
  onSubmit: (value: CreateChatFormValue) => void;
  busy?: boolean;
}

const INITIAL_VALUE: CreateChatFormValue = {
  name: '',
  members: [],
};

export const CreateChatForm = customize('CreateChatForm', (props: CreateChatFormProps) => {
  const { value = INITIAL_VALUE, onSubmit, busy = false } = props;
  const localizer = useLocalizer();

  return (
    <Form onFinish={onSubmit} layout="vertical" disabled={busy} initialValues={value}>
      <CreateChatNameInput name="name" />
      <CreateChatMemberInput name="members" />
      <Button loading={busy} type="primary" block htmlType="submit">
        {localizer.addChat.submitBtn()}
      </Button>
    </Form>
  );
});
