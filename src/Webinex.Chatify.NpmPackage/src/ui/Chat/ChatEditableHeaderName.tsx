import { Input, Tooltip } from 'antd';
import { chatifyApi } from '../../core';
import { customize } from '../customize';
import { useEffect, useState } from 'react';
import isHotkey from 'is-hotkey';
import { useConversation } from '../Conversation';

const isEscape = isHotkey('escape');
const isEnter = isHotkey('enter');

function useUpdateChatName(id: string, value: string) {
  const [updateChatName] = chatifyApi.useUpdateChatNameMutation();
  return () => updateChatName({ id, name: value });
}

export const ChatEditableHeaderName = customize('ChatEditableHeaderName', () => {
  const { id, name, active } = useConversation();
  const [editNameValue, setEditNameValue] = useState('');
  const [edit, setEdit] = useState(false);
  const updateChatName = useUpdateChatName(id, editNameValue);

  useEffect(() => {
    if (edit) {
      setEditNameValue(name ?? '');
    }
  }, [edit, name]);

  if (!active) {
    return <span className="wxchtf-conversation-name">{name}</span>;
  }

  if (edit) {
    return (
      <Tooltip defaultOpen title="Press Enter to save new chat name or Escape to cancel editing">
        <Input
          value={editNameValue}
          onChange={(e) => setEditNameValue(e.target.value)}
          onBlur={() => setEdit(false)}
          onKeyDown={(e) => {
            if (isEscape(e)) {
              setEdit(false);
            } else if (isEnter(e)) {
              setEdit(false);

              if (editNameValue !== name && editNameValue.trim() !== '') {
                updateChatName();
              }
            }
          }}
          autoFocus
        />
      </Tooltip>
    );
  }

  return (
    <Tooltip title="Click to edit name...">
      <span className="wxchtf-conversation-name --editable" onClick={() => setEdit(true)}>
        {name}
      </span>
    </Tooltip>
  );
});
