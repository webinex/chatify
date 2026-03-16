import { chatifyApi, MessageBase } from '@/core';
import { customize } from '../../customize';
import { useCallback } from 'react';
import { Button } from 'antd';
import { Icon } from '@/ui/common';
import { useChatContext } from '@/ui/Chat';

export interface MessageActionsProps {
  message: MessageBase;
}

export const MessageActions = customize('MessageActions', (props: MessageActionsProps) => {
  const { message } = props;
  const { id: chatId } = useChatContext();

  const [remove] = chatifyApi.useRemoveChatMessageMutation();
  const onDeleteClick = useCallback(
    () => remove({ chatId: chatId, messageId: message.id }).unwrap(),
    [remove],
  );

  return (
    <div className="wxchtf-message-actions">
      <Button
        className="wxchtf-remove-message"
        type="link"
        danger
        icon={<Icon type="delete-message" />}
        onClick={onDeleteClick}
      />
    </div>
  );
});
