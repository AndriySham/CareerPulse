import React from 'react';
import type { InterviewDto, InterviewType } from '@/types';
import {
  PhoneCall,
  Users,
  Code2,
  Cpu,
  Terminal,
  Award,
  Calendar,
  CheckCircle2,
  Clock,
  FileText,
  MessageSquare,
  Edit3,
  Trash2,
} from 'lucide-react';

export const INTERVIEW_TYPE_CONFIG: Record<
  InterviewType,
  {
    label: string;
    icon: React.ComponentType<{ className?: string }>;
    badgeClass: string;
  }
> = {
  PhoneScreen: {
    label: 'Phone Screen',
    icon: PhoneCall,
    badgeClass: 'text-sky-400 bg-sky-500/10 border-sky-500/20',
  },
  HRInterview: {
    label: 'HR Interview',
    icon: Users,
    badgeClass: 'text-indigo-400 bg-indigo-500/10 border-indigo-500/20',
  },
  TechnicalInterview: {
    label: 'Technical Interview',
    icon: Code2,
    badgeClass: 'text-purple-400 bg-purple-500/10 border-purple-500/20',
  },
  SystemDesign: {
    label: 'System Design',
    icon: Cpu,
    badgeClass: 'text-amber-400 bg-amber-500/10 border-amber-500/20',
  },
  CodingChallenge: {
    label: 'Coding Challenge',
    icon: Terminal,
    badgeClass: 'text-emerald-400 bg-emerald-500/10 border-emerald-500/20',
  },
  FinalRound: {
    label: 'Final Round',
    icon: Award,
    badgeClass: 'text-rose-400 bg-rose-500/10 border-rose-500/20',
  },
};

interface InterviewCardProps {
  interview: InterviewDto;
  onEdit: (interview: InterviewDto) => void;
  onDelete: (interview: InterviewDto) => void;
  isReadOnly?: boolean;
}

export const InterviewCard: React.FC<InterviewCardProps> = ({
  interview,
  onEdit,
  onDelete,
  isReadOnly = false,
}) => {
  const typeConfig = INTERVIEW_TYPE_CONFIG[interview.type] || {
    label: interview.type,
    icon: Code2,
    badgeClass: 'text-muted-foreground bg-accent border-border',
  };
  const TypeIcon = typeConfig.icon;

  const formatDateTime = (dateStr?: string | null): string | null => {
    if (!dateStr) return null;
    const date = new Date(dateStr);
    if (isNaN(date.getTime())) return null;
    return date.toLocaleString(undefined, {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const scheduledFormatted = formatDateTime(interview.scheduledAt);
  const conductedFormatted = formatDateTime(interview.conductedAt);

  const isUpcoming =
    interview.scheduledAt &&
    !interview.conductedAt &&
    new Date(interview.scheduledAt).getTime() > Date.now();

  return (
    <div className="group relative rounded-xl border border-border/70 bg-card p-4 shadow-sm hover:border-primary/40 transition-all space-y-3">
      {/* Header: Type Badge, Status & Actions */}
      <div className="flex items-start justify-between gap-3">
        <div className="flex flex-wrap items-center gap-2">
          <span
            className={`inline-flex items-center gap-1.5 rounded-lg border px-2.5 py-1 text-xs font-semibold ${typeConfig.badgeClass}`}
          >
            <TypeIcon className="h-3.5 w-3.5 shrink-0" />
            {typeConfig.label}
          </span>

          {interview.conductedAt ? (
            <span className="inline-flex items-center gap-1 rounded-full border border-emerald-500/30 bg-emerald-500/10 px-2 py-0.5 text-[11px] font-semibold text-emerald-400">
              <CheckCircle2 className="h-3 w-3" />
              Completed
            </span>
          ) : isUpcoming ? (
            <span className="inline-flex items-center gap-1 rounded-full border border-blue-500/30 bg-blue-500/10 px-2 py-0.5 text-[11px] font-semibold text-blue-400">
              <Clock className="h-3 w-3" />
              Upcoming
            </span>
          ) : null}
        </div>

        {!isReadOnly && (
          <div className="flex items-center gap-1 shrink-0">
            <button
              type="button"
              onClick={() => onEdit(interview)}
              className="p-1.5 rounded-lg text-muted-foreground hover:text-foreground hover:bg-accent transition-colors cursor-pointer"
              title="Edit Interview"
            >
              <Edit3 className="h-3.5 w-3.5" />
            </button>
            <button
              type="button"
              onClick={() => onDelete(interview)}
              className="p-1.5 rounded-lg text-muted-foreground/60 hover:text-destructive hover:bg-destructive/10 transition-colors cursor-pointer"
              title="Delete Interview"
            >
              <Trash2 className="h-3.5 w-3.5" />
            </button>
          </div>
        )}
      </div>

      {/* Date Information */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 text-xs text-muted-foreground">
        {scheduledFormatted && (
          <div className="flex items-center gap-1.5">
            <Calendar className="h-3.5 w-3.5 text-primary shrink-0" />
            <span>
              Scheduled:{' '}
              <strong className="text-foreground font-medium">{scheduledFormatted}</strong>
            </span>
          </div>
        )}
        {conductedFormatted && (
          <div className="flex items-center gap-1.5">
            <CheckCircle2 className="h-3.5 w-3.5 text-emerald-400 shrink-0" />
            <span>
              Conducted:{' '}
              <strong className="text-foreground font-medium">{conductedFormatted}</strong>
            </span>
          </div>
        )}
      </div>

      {/* Notes */}
      {interview.notes && (
        <div className="rounded-lg border border-border/50 bg-background/50 p-2.5 space-y-1">
          <div className="flex items-center gap-1 text-[11px] font-semibold text-muted-foreground">
            <FileText className="h-3 w-3 text-primary" />
            <span>Notes</span>
          </div>
          <p className="text-xs text-foreground/90 whitespace-pre-wrap leading-relaxed">
            {interview.notes}
          </p>
        </div>
      )}

      {/* Feedback & Outcomes */}
      {interview.feedback && (
        <div className="rounded-lg border border-primary/20 bg-primary/5 p-2.5 space-y-1">
          <div className="flex items-center gap-1 text-[11px] font-semibold text-primary">
            <MessageSquare className="h-3 w-3 text-primary" />
            <span>Feedback & Outcome</span>
          </div>
          <p className="text-xs text-foreground/90 whitespace-pre-wrap leading-relaxed">
            {interview.feedback}
          </p>
        </div>
      )}
    </div>
  );
};

export default InterviewCard;
