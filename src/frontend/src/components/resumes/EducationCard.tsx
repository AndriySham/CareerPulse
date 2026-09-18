import React from 'react';
import type { EducationDto } from '@/types';
import { GraduationCap, Calendar, Edit3, Trash2 } from 'lucide-react';

interface EducationCardProps {
  education: EducationDto;
  onEdit: (education: EducationDto) => void;
  onDelete: (education: EducationDto) => void;
  isReadOnly?: boolean;
}

export const EducationCard: React.FC<EducationCardProps> = ({
  education,
  onEdit,
  onDelete,
  isReadOnly = false,
}) => {
  const formatPeriod = (start?: number | null, end?: number | null): string | null => {
    if (start && end) {
      return `${start} — ${end}`;
    }
    if (start) {
      return `Since ${start}`;
    }
    if (end) {
      return `Completed ${end}`;
    }
    return null;
  };

  const periodText = formatPeriod(education.startYear, education.endYear);

  return (
    <div className="group relative rounded-xl border border-border/80 bg-background p-4 shadow-xs hover:border-border transition-all flex flex-col justify-between gap-3">
      <div className="flex items-start justify-between gap-3">
        <div className="flex items-start gap-3">
          <div className="p-2 rounded-lg bg-primary/10 text-primary border border-primary/20 shrink-0 mt-0.5">
            <GraduationCap className="h-4 w-4" />
          </div>
          <div className="space-y-1">
            <h3 className="font-semibold text-sm text-foreground leading-snug">
              {education.institutionName}
            </h3>

            {education.description && (
              <p className="text-xs text-muted-foreground whitespace-pre-line leading-relaxed">
                {education.description}
              </p>
            )}

            {periodText && (
              <div className="flex items-center gap-1.5 text-[11px] font-medium text-muted-foreground/80 pt-0.5">
                <Calendar className="h-3 w-3 text-primary shrink-0" />
                <span>{periodText}</span>
              </div>
            )}
          </div>
        </div>

        {/* Action buttons (only when not read-only) */}
        {!isReadOnly && (
          <div className="flex items-center gap-1 shrink-0">
            <button
              type="button"
              onClick={() => onEdit(education)}
              className="p-1.5 rounded-lg text-muted-foreground hover:text-foreground hover:bg-accent transition-colors cursor-pointer"
              title="Edit Education"
            >
              <Edit3 className="h-3.5 w-3.5" />
            </button>
            <button
              type="button"
              onClick={() => onDelete(education)}
              className="p-1.5 rounded-lg text-muted-foreground/60 hover:text-destructive hover:bg-destructive/10 transition-colors cursor-pointer"
              title="Delete Education"
            >
              <Trash2 className="h-3.5 w-3.5" />
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default EducationCard;
