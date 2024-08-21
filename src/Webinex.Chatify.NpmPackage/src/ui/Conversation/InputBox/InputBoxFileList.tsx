import { File } from '../../../core';
import { customize } from '../../customize';
import { InputBoxFile } from './InputBoxFile';

export interface InputBoxFileListProps {
  files: File[];
  disabled?: boolean;
  onDelete: (file: File) => void;
}

export const InputBoxFileList = customize('InputBoxFileList', (props: InputBoxFileListProps) => {
  const { files, onDelete, disabled } = props;

  return (
    <div className="wxchtf-file-list">
      {files.map((file) => (
        <InputBoxFile key={file.ref} file={file} onDelete={onDelete} disabled={disabled} />
      ))}
    </div>
  );
});
