import React from 'react';
import type { RevisionStatus } from '@/types';
import { Edit3, Lock } from 'lucide-react';

interface ResumeStatusBadgeProps {
  status: RevisionStatus;
  size?: 'sm' | 'md';
}

export const ResumeStatusBadge: React.FC<ResumeStatusBadgeProps> = ({
  status,
  size = 'md',
}) => {
  const isDraft = status === 'Draft';

  const sizeClasses = {
    sm: 'px-2 py-0.5 text-xs gap-1',
    md: 'px-2.5 py-1 text-xs gap-1.5 font-semibold',
  }[size];

  const iconSize = size === 'sm' ? 'h-3 w-3' : 'h-3.5 w-3.5';

  if (isDraft) {
    return (
      <span
        className={`inline-flex items-center rounded-full border border-amber-500/30 bg-amber-500/10 text-amber-400 font-medium ${sizeClasses}`}
      >
        <Edit3 className={`${iconSize} shrink-0`} />
        Draft (Editable)
      </span>
    );
  }

  return (
    <span
      className={`inline-flex items-center rounded-full border border-emerald-500/30 bg-emerald-500/10 text-emerald-400 font-medium ${sizeClasses}`}
    >
      <Lock className={`${iconSize} shrink-0`} />
      Applied (Immutable)
    </span>
  );
};

export default ResumeStatusBadge;
