import { FC } from 'react';
import { customize } from '../customize';
import {
  SendOutlined,
  PaperClipOutlined,
  UserAddOutlined,
  UsergroupAddOutlined,
  UserDeleteOutlined,
  UsergroupDeleteOutlined,
  TeamOutlined,
  EditOutlined,
  CloseOutlined,
  EyeInvisibleOutlined,
  EyeOutlined,
  DeleteOutlined,
  DownloadOutlined,
  SettingOutlined,
} from '@ant-design/icons';

export type IconType = keyof typeof ICONS;

export interface IconProps {
  type: IconType;
}

export const ICONS = {
  attach: PaperClipOutlined,
  send: SendOutlined,
  'remove-member': UserDeleteOutlined,
  'remove-member-delete-history': UsergroupDeleteOutlined,
  'add-member': UserAddOutlined,
  'add-member-with-history': UsergroupAddOutlined,
  members: TeamOutlined,
  edit: EditOutlined,
  close: CloseOutlined,
  chat: TeamOutlined,
  watch: EyeOutlined,
  unwatch: EyeInvisibleOutlined,
  'delete-file': DeleteOutlined,
  'open-file': DownloadOutlined,
  settings: SettingOutlined,
};

export const Icon = customize<FC<IconProps>>('Icon', (props) => {
  const { type } = props;
  const IconComponent = ICONS[type];
  return IconComponent ? <IconComponent /> : null;
});
