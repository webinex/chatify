import { useCallback, useEffect, useLayoutEffect, useRef, useState } from 'react';
import { InputFilesBox } from './InputFilesBox';
import { File } from '../../../core';
import { InputTextBox } from './InputTextBox';
import { customize, useCustomizeContext } from '../../customize';
import { InputSubmitButtonBox } from './InputSubmitButtonBox';
import { useConversation } from '../ConversationContext';
import { ConversationCustomizeValue } from '../Conversation';
import { InputBoxFileList } from './InputBoxFileList';

function useFormState() {
  const { onSend } = useConversation();
  const [text, setText] = useState('');
  const { onUpload, files, loading, setFiles, showFileBox, onDeleteFile } = useFlippoInputFileBox();

  const onSubmit = useCallback(() => {
    if (text.length === 0 || loading) {
      return;
    }

    setText('');
    setFiles([]);
    Promise.resolve(onSend({ text, files })).catch(() => {
      setText(text);
      setFiles(files);
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [text, onSend, files, setFiles, loading]);

  return [onSubmit, { text, setText, files, onUpload, loading, showFileBox, onDeleteFile }] as const;
}

function useFlippoInputFileBox() {
  const [loading, setLoading] = useState(false);
  const [files, setFiles] = useState<File[]>([]);
  const { flippo } = useCustomizeContext<ConversationCustomizeValue>();

  const onUpload = useCallback(
    (files: globalThis.File[]) => {
      if (!flippo) return;

      setLoading(true);
      Promise.all(files.map((file) => flippo.store(file)))
        .then((references) =>
          setFiles((prev) => [
            ...prev,
            ...references.map((reference, index) => ({
              name: files[index].name,
              ref: reference,
              bytes: files[index].size,
            })),
          ]),
        )
        .finally(() => setLoading(false));
    },
    [flippo],
  );

  const onDeleteFile = useCallback((file: File) => {
    setFiles((prev) => prev.filter((f) => f !== file));
  }, []);

  return { onUpload, files, loading, setFiles, showFileBox: !!flippo, onDeleteFile };
}

export const InputBox = customize('InputBox', () => {
  const [onSend, { text, setText, files, loading, onUpload, showFileBox, onDeleteFile }] = useFormState();
  const inputRef = useRef<HTMLTextAreaElement>(null);
  const [showFilesModal, setShowFilesModal] = useState(false);
  const [showFileList, setShowFileList] = useState(false);
  const toggleShowFileList = useCallback(() => setShowFileList((prev) => !prev), []);

  useLayoutEffect(() => inputRef.current?.focus(), []);

  useEffect(() => {
    if (files.length === 0 && showFilesModal) {
      setShowFilesModal(false);
    }
  }, [files, showFilesModal]);

  return (
    <div className="wxchtf-input-form">
      <div className="wxchtf-input-box">
        {showFileList && (
          <div className="wxchtf-file-list-box">
            <InputBoxFileList files={files} onDelete={onDeleteFile} />
          </div>
        )}
        <div className="wxchtf-input-box-row">
          {showFileBox && (
            <InputFilesBox
              onAddedClick={toggleShowFileList}
              onUpload={onUpload}
              value={files}
              loading={loading}
            />
          )}
          <InputTextBox value={text} onSend={onSend} onChange={setText} inputRef={inputRef} />
          <InputSubmitButtonBox onSubmit={onSend} disabled={loading} />
        </div>
      </div>
    </div>
  );
});
