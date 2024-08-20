import { FC } from 'react';
import { customize } from '../customize';
import SendOutlined from '@ant-design/icons/SendOutlined';
import PaperClipOutlined from '@ant-design/icons/PaperClipOutlined';
import UserAddOutlined from '@ant-design/icons/UserAddOutlined';
import UsergroupAddOutlined from '@ant-design/icons/UsergroupAddOutlined';
import UserDeleteOutlined from '@ant-design/icons/UserDeleteOutlined';
import UsergroupDeleteOutlined from '@ant-design/icons/UsergroupDeleteOutlined';
import TeamOutlined from '@ant-design/icons/TeamOutlined';
import EditOutlined from '@ant-design/icons/EditOutlined';
import CloseOutlined from '@ant-design/icons/CloseOutlined';
import EyeInvisibleOutlined from '@ant-design/icons/EyeInvisibleOutlined';
import EyeOutlined from '@ant-design/icons/EyeOutlined';
import DeleteOutlined from '@ant-design/icons/DeleteOutlined';
import DownloadOutlined from '@ant-design/icons/DownloadOutlined';

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
};

export const Icon = customize<FC<IconProps>>('Icon', (props) => {
  const { type } = props;
  const IconComponent = ICONS[type];
  return IconComponent ? <IconComponent /> : null;
});
