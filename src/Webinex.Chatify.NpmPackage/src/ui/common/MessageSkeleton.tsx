import { Skeleton } from 'antd';
import { customize } from '../customize';

export interface MessageSkeletonProps {
  count?: number;
}

export const MessageSkeleton = customize('MessageSkeleton', (props: MessageSkeletonProps) => {
  const { count = 1 } = props;
  return (
    <>
      {Array.from(Array(count)).map((_, index) => (
        <div className="wxchtf-message-skeleton" key={index}>
          <Skeleton.Avatar className="wxchtf-avatar" active />
          <Skeleton.Button active className="wxchtf-content" />
        </div>
      ))}
    </>
  );
});
