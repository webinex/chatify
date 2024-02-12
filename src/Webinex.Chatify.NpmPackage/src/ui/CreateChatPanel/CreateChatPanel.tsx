import { useAddChatMutation } from '../../core';
import { useCallback } from 'react';
import { uniqId } from '../../util';
import { useLocalizer } from '../localizer';
import { CreateChatForm, CreateChatFormValue } from './CreateChatForm';
import { customize } from '../customize';

function useSubmit() {
  const [add, { isFetching }] = useAddChatMutation();
  const onSubmit = useCallback(
    (value: CreateChatFormValue) =>
      add({
        name: value.name,
        members: value.members,
        requestId: uniqId(),
      }),
    [add],
  );

  return [onSubmit, isFetching] as const;
}

export const CreateChatPanel = customize('CreateChatPanel', () => {
  const localizer = useLocalizer();
  const [onSubmit, submitting] = useSubmit();

  return (
    <div className="wxchtf-create-chat">
      <h3>{localizer.addChat.title()}</h3>
      <CreateChatForm busy={submitting ?? false} onSubmit={onSubmit} />
    </div>
  );
});
