import { useCallback, useState } from 'react';
import { useSelect, useSendMutation } from '../../core';
import { uniqId } from '../../util';
import { InputFilesBox } from './InputFilesBox';
import { InputTextBox } from './InputTextBox';
import { customize } from '../customize';
import { InputSubmitButtonBox } from './InputSubmitButtonBox';

function useSend(text: string, setText: (value: string) => void) {
  const chatId = useSelect((x) => x.ui.chatId!, []);
  const [add] = useSendMutation();

  return useCallback(() => {
    if (text.length === 0) {
      return;
    }

    setText('');
    add({ chatId, text: text, files: [], requestId: uniqId() });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chatId, text]);
}

export const InputBox = customize('InputBox', () => {
  const [text, setText] = useState('');
  const onSend = useSend(text, setText);

  return (
    <div className="wxchtf-input-form">
      <div className="wxchtf-input-box">
        <InputFilesBox />
        <InputTextBox value={text} onSend={onSend} onChange={setText} />
        <InputSubmitButtonBox onSubmit={onSend} />
      </div>
    </div>
  );
});
