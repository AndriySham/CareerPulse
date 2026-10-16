import React from 'react';
import type { ApplicationCommunicationDto } from '@/types';
import { MessageSquare, Calendar, Edit3, Trash2 } from 'lucide-react';

interface CommunicationCardProps {
  communication: ApplicationCommunicationDto;
  onEdit: (communication: ApplicationCommunicationDto) => void;
  onDelete: (communication: ApplicationCommunicationDto) => void;
  isReadOnly?: boolean;
}

export const CommunicationCard: React.FC<CommunicationCardProps> = ({
  communication,
  onEdit,
  onDelete,
  isReadOnly = false,
}) => {
  const formattedOccurredDate = (() => {
    const date = new Date(communication.occurredAt);
    if (isNaN(date.getTime())) return communication.occurredAt;
    return date.toLocaleString(undefined, {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  })();

  return (
    <div className="group relative rounded-xl border border-border/70 bg-card p-4 shadow-sm hover:border-primary/40 transition-all space-y-2.5">
      {/* Header: Date and Actions */}
      <div className="flex items-center justify-between gap-3">
        <div className="flex items-center gap-2">
          <div className="p-1.5 rounded-lg bg-primary/10 text-primary border border-primary/20 shrink-0">
            <MessageSquare className="h-3.5 w-3.5" />
          </div>
          <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
            <Calendar className="h-3 w-3 text-primary" />
            <span className="font-semibold text-foreground">{formattedOccurredDate}</span>
          </div>
        </div>

        {!isReadOnly && (
          <div className="flex items-center gap-1 shrink-0">
            <button
              type="button"
              onClick={() => onEdit(communication)}
              className="p-1.5 rounded-lg text-muted-foreground hover:text-foreground hover:bg-accent transition-colors cursor-pointer"
              title="Edit Communication"
            >
              <Edit3 className="h-3.5 w-3.5" />
            </button>
            <button
              type="button"
              onClick={() => onDelete(communication)}
              className="p-1.5 rounded-lg text-muted-foreground/60 hover:text-destructive hover:bg-destructive/10 transition-colors cursor-pointer"
              title="Delete Communication"
            >
              <Trash2 className="h-3.5 w-3.5" />
            </button>
          </div>
        )}
      </div>

      {/* Notes Body */}
      <p className="text-xs text-foreground/90 whitespace-pre-wrap leading-relaxed pl-7">
        {communication.notes}
      </p>
    </div>
  );
};

export default CommunicationCard;
