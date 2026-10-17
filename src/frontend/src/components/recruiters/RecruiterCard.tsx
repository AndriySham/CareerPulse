import React from 'react';
import type { RecruiterDto } from '@/types';
import {
  UserCheck,
  Mail,
  Phone,
  Send,
  Linkedin,
  Edit3,
  Trash2,
  ExternalLink,
  FileText,
} from 'lucide-react';

interface RecruiterCardProps {
  recruiter: RecruiterDto;
  onEdit?: (recruiter: RecruiterDto) => void;
  onDelete?: (recruiter: RecruiterDto) => void;
  isReadOnly?: boolean;
}

export const RecruiterCard: React.FC<RecruiterCardProps> = ({
  recruiter,
  onEdit,
  onDelete,
  isReadOnly = false,
}) => {
  // Format Telegram URL for href
  const telegramHref = recruiter.telegramUrl
    ? recruiter.telegramUrl.startsWith('http://') || recruiter.telegramUrl.startsWith('https://')
      ? recruiter.telegramUrl
      : `https://t.me/${recruiter.telegramUrl.replace(/^@/, '')}`
    : null;

  // Format LinkedIn URL for href
  const linkedInHref = recruiter.linkedInUrl
    ? recruiter.linkedInUrl.startsWith('http://') || recruiter.linkedInUrl.startsWith('https://')
      ? recruiter.linkedInUrl
      : `https://${recruiter.linkedInUrl}`
    : null;

  return (
    <div className="group relative rounded-xl border border-border/70 bg-card p-4 shadow-sm hover:border-primary/40 transition-all space-y-3">
      {/* Top Header: Avatar/Icon, Name, and Actions */}
      <div className="flex items-start justify-between gap-3">
        <div className="flex items-center gap-2.5 min-w-0">
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary border border-primary/20">
            <UserCheck className="h-4 w-4" />
          </div>
          <div className="min-w-0">
            <h4 className="font-bold text-sm text-foreground tracking-tight truncate">
              {recruiter.name}
            </h4>
            <span className="text-[11px] text-muted-foreground font-medium">
              Company Recruiter / Talent Acquisition
            </span>
          </div>
        </div>

        {!isReadOnly && (
          <div className="flex items-center gap-1 shrink-0">
            {onEdit && (
              <button
                type="button"
                onClick={() => onEdit(recruiter)}
                className="p-1.5 rounded-lg text-muted-foreground hover:text-foreground hover:bg-accent transition-colors cursor-pointer"
                title="Edit Recruiter"
              >
                <Edit3 className="h-3.5 w-3.5" />
              </button>
            )}
            {onDelete && (
              <button
                type="button"
                onClick={() => onDelete(recruiter)}
                className="p-1.5 rounded-lg text-muted-foreground/60 hover:text-destructive hover:bg-destructive/10 transition-colors cursor-pointer"
                title="Delete Recruiter"
              >
                <Trash2 className="h-3.5 w-3.5" />
              </button>
            )}
          </div>
        )}
      </div>

      {/* Contact Channels Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 text-xs pt-1">
        {recruiter.email && (
          <a
            href={`mailto:${recruiter.email}`}
            className="flex items-center gap-2 rounded-lg bg-accent/40 px-2.5 py-1.5 text-foreground hover:bg-accent hover:text-primary transition-colors truncate"
            title={`Send email to ${recruiter.email}`}
          >
            <Mail className="h-3.5 w-3.5 text-primary shrink-0" />
            <span className="truncate">{recruiter.email}</span>
          </a>
        )}

        {recruiter.phone && (
          <a
            href={`tel:${recruiter.phone}`}
            className="flex items-center gap-2 rounded-lg bg-accent/40 px-2.5 py-1.5 text-foreground hover:bg-accent hover:text-primary transition-colors truncate"
            title={`Call ${recruiter.phone}`}
          >
            <Phone className="h-3.5 w-3.5 text-primary shrink-0" />
            <span className="truncate">{recruiter.phone}</span>
          </a>
        )}

        {telegramHref && (
          <a
            href={telegramHref}
            target="_blank"
            rel="noopener noreferrer"
            className="flex items-center justify-between gap-1.5 rounded-lg bg-accent/40 px-2.5 py-1.5 text-foreground hover:bg-accent hover:text-primary transition-colors truncate"
            title="Open Telegram Chat"
          >
            <div className="flex items-center gap-2 truncate">
              <Send className="h-3.5 w-3.5 text-sky-400 shrink-0" />
              <span className="truncate">Telegram</span>
            </div>
            <ExternalLink className="h-3 w-3 text-muted-foreground shrink-0" />
          </a>
        )}

        {linkedInHref && (
          <a
            href={linkedInHref}
            target="_blank"
            rel="noopener noreferrer"
            className="flex items-center justify-between gap-1.5 rounded-lg bg-accent/40 px-2.5 py-1.5 text-foreground hover:bg-accent hover:text-primary transition-colors truncate"
            title="Open LinkedIn Profile"
          >
            <div className="flex items-center gap-2 truncate">
              <Linkedin className="h-3.5 w-3.5 text-[#0A66C2] shrink-0" />
              <span className="truncate">LinkedIn</span>
            </div>
            <ExternalLink className="h-3 w-3 text-muted-foreground shrink-0" />
          </a>
        )}
      </div>

      {/* Recruiter Notes */}
      {recruiter.notes ? (
        <div className="rounded-lg border border-border/40 bg-muted/20 p-2.5 text-xs">
          <div className="flex items-center gap-1.5 text-[11px] font-semibold text-muted-foreground mb-1">
            <FileText className="h-3 w-3 text-primary/70" />
            <span>Notes</span>
          </div>
          <p className="text-muted-foreground whitespace-pre-wrap leading-relaxed line-clamp-3">
            {recruiter.notes}
          </p>
        </div>
      ) : null}
    </div>
  );
};

export default RecruiterCard;
