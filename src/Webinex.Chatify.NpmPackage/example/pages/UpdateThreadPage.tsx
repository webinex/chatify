import { Button, Card, Form, Input } from 'antd';
import { useState } from 'react';
import { useCallback } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

export interface UpdateThreadPageProps {
  me: string;
}

function useSubmit(props: UpdateThreadPageProps, id: string | undefined) {
  const { me } = props;
  const [submitting, setSubmitting] = useState(false);
  const navigate = useNavigate();

  const onSubmit = useCallback(
    (formValue: any) => {
      if (!id) {
        return;
      }

      setSubmitting(true);
      fetch('/api/thread', {
        method: 'PUT',
        body: JSON.stringify({ id, name: formValue.name }),
        headers: {
          'content-type': 'application/json',
          'X-USER-ID': me,
        },
      })
        .then(() => navigate(`/thread/${id}`))
        .finally(() => setSubmitting(false));
    },
    [me, navigate, id],
  );

  return [onSubmit, { submitting }] as const;
}

const INITIAL_VALUE = {
  name: '',
};

export function UpdateThreadPage(props: UpdateThreadPageProps) {
  const { id } = useParams<'id'>();
  const [onSubmit, { submitting }] = useSubmit(props, id);

  return (
    <Card style={{ margin: 'auto' }} className="update-thread-page">
      <Form onFinish={onSubmit} layout="vertical" disabled={submitting} initialValues={INITIAL_VALUE}>
        <Form.Item
          name="name"
          label="Name"
          rules={[
            { required: true, message: 'This field is required' },
            { max: 250, message: 'Might be less than 250 chars' },
          ]}
        >
          <Input />
        </Form.Item>
        <Button loading={submitting} type="primary" block htmlType="submit">
          Submit
        </Button>
      </Form>
    </Card>
  );
}
