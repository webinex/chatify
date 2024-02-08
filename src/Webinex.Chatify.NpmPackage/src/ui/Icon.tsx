import { FC } from 'react';
import { customize } from './customize';
import SendOutlined from '@ant-design/icons/SendOutlined';
import PaperClipOutlined from '@ant-design/icons/PaperClipOutlined';
import UserAddOutlined from '@ant-design/icons/UserAddOutlined';
import UsergroupAddOutlined from '@ant-design/icons/UsergroupAddOutlined';
import UserDeleteOutlined from '@ant-design/icons/UserDeleteOutlined';
import UsergroupDeleteOutlined from '@ant-design/icons/UsergroupDeleteOutlined';
import TeamOutlined from '@ant-design/icons/TeamOutlined';
import EditOutlined from '@ant-design/icons/EditOutlined';

export type IconType = keyof typeof ICONS;

export interface IconProps {
  type: IconType;
}

export const ICONS = {
  attach: PaperClipOutlined,
  send: SendOutlined,
  remove: UserDeleteOutlined,
  remove_delete_history: UsergroupDeleteOutlined,
  add: UserAddOutlined,
  add_with_history: UsergroupAddOutlined,
  team: TeamOutlined,
  edit: EditOutlined,
};

export const Icon = customize<FC<IconProps>>('Icon', (props) => {
  const { type } = props;
  const IconComponent = ICONS[type];
  return IconComponent ? <IconComponent /> : null;
});
