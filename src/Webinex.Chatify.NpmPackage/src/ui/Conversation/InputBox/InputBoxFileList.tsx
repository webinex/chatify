import { File } from '../../../core';
import { customize } from '../../customize';
import { InputBoxFile } from './InputBoxFile';

export interface InputBoxFileListProps {
  files: File[];
  onDelete: (file: File) => void;
}

export const InputBoxFileList = customize('InputBoxFileList', (props: InputBoxFileListProps) => {
  const { files, onDelete } = props;
  return (
    <div className="wxchtf-file-list">
      {files.map((file) => (
        <InputBoxFile key={file.ref} file={file} onDelete={onDelete} />
      ))}
    </div>
  );
});
