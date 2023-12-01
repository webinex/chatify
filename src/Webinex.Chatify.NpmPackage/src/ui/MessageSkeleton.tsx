import { Skeleton } from 'antd';

export interface MessageSkeletonProps {
  count?: number;
}

export function MessageSkeleton(props: MessageSkeletonProps) {
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
}
