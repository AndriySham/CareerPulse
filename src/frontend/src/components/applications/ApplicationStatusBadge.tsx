import React from 'react';
import type { ApplicationStatus } from '@/types';
import {
  FileEdit,
  Send,
  Eye,
  Users,
  Code2,
  Award,
  XCircle,
  Clock,
} from 'lucide-react';

interface ApplicationStatusBadgeProps {
  status: ApplicationStatus;
  size?: 'sm' | 'md' | 'lg';
  showIcon?: boolean;
}

export const STATUS_CONFIG: Record<
  ApplicationStatus,
  {
    label: string;
    description: string;
    badgeClass: string;
    icon: React.ComponentType<{ className?: string }>;
  }
> = {
  Draft: {
    label: 'Draft',
    description: 'Application prepared but not yet submitted.',
    badgeClass: 'bg-slate-500/10 text-slate-400 border-slate-500/30',
    icon: FileEdit,
  },
  Applied: {
    label: 'Applied',
    description: 'Submitted to employer.',
    badgeClass: 'bg-blue-500/10 text-blue-400 border-blue-500/30',
    icon: Send,
  },
  Viewed: {
    label: 'Viewed',
    description: 'Viewed by employer/recruiter.',
    badgeClass: 'bg-sky-500/10 text-sky-400 border-sky-500/30',
    icon: Eye,
  },
  HRInterview: {
    label: 'HR Interview',
    description: 'Screening or HR interview scheduled/completed.',
    badgeClass: 'bg-indigo-500/10 text-indigo-400 border-indigo-500/30',
    icon: Users,
  },
  TechnicalInterview: {
    label: 'Tech Interview',
    description: 'Technical interview or coding assessment.',
    badgeClass: 'bg-purple-500/10 text-purple-400 border-purple-500/30',
    icon: Code2,
  },
  Offer: {
    label: 'Offer',
    description: 'Job offer received!',
    badgeClass: 'bg-emerald-500/10 text-emerald-400 border-emerald-500/30',
    icon: Award,
  },
  Rejected: {
    label: 'Rejected',
    description: 'Application declined by employer.',
    badgeClass: 'bg-rose-500/10 text-rose-400 border-rose-500/30',
    icon: XCircle,
  },
  NoResponse: {
    label: 'No Response',
    description: 'No update from employer after submission.',
    badgeClass: 'bg-amber-500/10 text-amber-400 border-amber-500/30',
    icon: Clock,
  },
};

export const ApplicationStatusBadge: React.FC<ApplicationStatusBadgeProps> = ({
  status,
  size = 'md',
  showIcon = true,
}) => {
  const config = STATUS_CONFIG[status] || STATUS_CONFIG.Draft;
  const Icon = config.icon;

  const sizeClasses = {
    sm: 'px-2 py-0.5 text-[11px] gap-1',
    md: 'px-2.5 py-1 text-xs gap-1.5',
    lg: 'px-3 py-1.5 text-sm gap-2 font-medium',
  }[size];

  const iconSizes = {
    sm: 'h-3 w-3',
    md: 'h-3.5 w-3.5',
    lg: 'h-4 w-4',
  }[size];

  return (
    <span
      className={`inline-flex items-center rounded-full font-semibold border ${config.badgeClass} ${sizeClasses}`}
      title={config.description}
    >
      {showIcon && <Icon className={`${iconSizes} shrink-0`} />}
      <span>{config.label}</span>
    </span>
  );
};

export default ApplicationStatusBadge;
