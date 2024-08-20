import { Button, Card, Form, Input } from 'antd';
import React, { useState } from 'react';
import { useCallback } from 'react';
import { useNavigate } from 'react-router-dom';

export interface CreateThreadPageProps {
  me: string;
}

function useSubmit(props: CreateThreadPageProps) {
  const { me } = props;
  const [submitting, setSubmitting] = useState(false);
  const navigate = useNavigate();

  const onSubmit = useCallback(
    (formValue: any) => {
      setSubmitting(true);
      fetch('/api/thread', {
        method: 'POST',
        body: JSON.stringify({ name: formValue.name }),
        headers: {
          'content-type': 'application/json',
          'X-USER-ID': me,
        },
      })
        .then((response) => response.text())
        .then((id) => navigate(`/thread/${id}`))
        .finally(() => setSubmitting(false));
    },
    [me, navigate],
  );

  return [onSubmit, { submitting }] as const;
}

const INITIAL_VALUE = {
  name: '',
};

export function CreateThreadPage(props: CreateThreadPageProps) {
  const [onSubmit, { submitting }] = useSubmit(props);

  return (
    <Card style={{ margin: 'auto' }} className="create-thread-page">
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
