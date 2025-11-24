import { Avatar } from './Avatar';
import { Icon } from './Icon';
import { MessageSkeleton } from './MessageSkeleton';

export * from './Avatar';
export * from './Icon';
export * from './MessageSkeleton';
export * from './ChatList';

export interface CommonCustomizeValue {
  Avatar?: typeof Avatar.Component | null;
  Icon?: typeof Icon.Component | null;
  MessageSkeleton?: typeof MessageSkeleton.Component | null;
}
