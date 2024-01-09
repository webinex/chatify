import { Button, Input } from 'antd';
import { useCallback, useState } from 'react';
import isHotkey from 'is-hotkey';
import { useAddMessage } from '../core/useAddMessage';
import { uniqId } from '../util';
import { useSelect } from '../core';
import { useLocalizer } from './localizer';
import { Icon } from './Icon';

const isSend = isHotkey('ctrl+enter');

export function InputForm() {
  const chatId = useSelect((x) => x.ui.chatId!, []);
  const [add] = useAddMessage({ chatId });
  const [text, setText] = useState('');
  const localizer = useLocalizer();

  const onSend = useCallback(() => {
    setText('');
    add({ text: text, files: [], requestId: uniqId() });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chatId, text]);

  return (
    <div className="wxchtf-input-form">
      <div className="wxchtf-input-box">
        <div className="wxchtf-add-files-box">
          <Button type="link" icon={<Icon type="attach" />} className="wxchtf-add-files-btn" />
        </div>
        <div className="wxchtf-text-input-box">
          <Input.TextArea
            value={text}
            onChange={(e) => setText(e.target.value)}
            autoSize={{ maxRows: 5 }}
            className="wxchtf-text-input-textarea"
            placeholder={localizer.input.placeholder()}
            onKeyDown={(event) => {
              if (isSend(event)) {
                onSend();
              }
            }}
          />
        </div>
        <div className="wxchtf-submit-btn-box">
          <Button onClick={onSend} type="link" icon={<Icon type="send" />} className="wxchtf-submit-btn" />
        </div>
      </div>
    </div>
  );
}
