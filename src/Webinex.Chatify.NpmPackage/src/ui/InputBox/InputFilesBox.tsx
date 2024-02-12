import { Button } from 'antd';
import { customize } from '../customize';
import { Icon } from '../Icon';

export const InputFilesBox = customize('InputFilesBox', () => (
  <div className="wxchtf-add-files-box">
    <Button type="link" icon={<Icon type="attach" />} className="wxchtf-add-files-btn" />
  </div>
));
