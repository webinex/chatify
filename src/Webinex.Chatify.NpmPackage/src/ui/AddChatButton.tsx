import { Button } from 'antd';
import { useDispatch } from '../core';
import { FC, useCallback } from 'react';
import { customize } from '../util';

const _AddChatButton: FC = () => {
  const dispatch = useDispatch();
  const onAddChat = useCallback(() => dispatch({ type: 'new_chat_open', data: undefined }), [dispatch]);

  return (
    <Button onClick={onAddChat} icon={'+'} type="link">
      Add chat
    </Button>
  );
};

_AddChatButton.displayName = 'AddChatButton';

export const AddChatButton = customize('AddChatButton', _AddChatButton);
