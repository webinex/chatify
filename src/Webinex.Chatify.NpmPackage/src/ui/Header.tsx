import { FC } from 'react';
import { AddChatButton } from './AddChatButton';
import { customize } from '../util';

const _Header: FC = () => {
  return (
    <div className="wxchtf-header">
      <AddChatButton />
    </div>
  );
};

_Header.displayName = 'Header';
export const Header = customize('Header', _Header);
