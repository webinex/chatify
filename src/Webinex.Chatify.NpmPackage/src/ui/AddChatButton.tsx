import { Button } from 'antd';
import { useDispatch } from '../core';
import { useCallback } from 'react';
import { customize } from './customize';

export const AddChatButton = customize('AddChatButton', () => {
  const dispatch = useDispatch();
  const onAddChat = useCallback(() => dispatch({ type: 'new_chat_open', data: undefined }), [dispatch]);

  return (
    <Button onClick={onAddChat} icon={'+'} type="link">
      Add chat
    </Button>
  );
});
