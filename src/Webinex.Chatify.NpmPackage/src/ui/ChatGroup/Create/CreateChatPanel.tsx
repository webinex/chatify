import { chatifyApi } from '../../../core';
import { useCallback } from 'react';
import { useLocalizer } from '../../localizer';
import { CreateChatForm, CreateChatFormValue } from './CreateChatForm';
import { customize } from '../../customize';
import { Button } from 'antd';
import { Icon } from '../../common/Icon';
import { useChatGroupContext } from '../ChatGroupContext';

function useSubmit() {
  const [add, { isLoading }] = chatifyApi.useAddChatMutation();
  const { openChat } = useChatGroupContext();

  const onSubmit = useCallback(
    (value: CreateChatFormValue) =>
      add({
        name: value.name,
        members: value.members,
      })
        .unwrap()
        .then((id) => openChat(id)),
    [add, openChat],
  );

  return [onSubmit, isLoading] as const;
}

export const CreateChatPanel = customize('CreateChatPanel', () => {
  const localizer = useLocalizer();
  const [onSubmit, submitting] = useSubmit();
  const { toggleNewChat } = useChatGroupContext();

  return (
    <div className="wxchtf-create-chat-panel">
      <Button
        onClick={() => toggleNewChat()}
        className="wxchtf-create-chat-panel-close"
        type="link"
        icon={<Icon type="close" />}
      />
      <h3>{localizer.addChat.title()}</h3>
      <CreateChatForm busy={submitting ?? false} onSubmit={onSubmit} />
    </div>
  );
});
