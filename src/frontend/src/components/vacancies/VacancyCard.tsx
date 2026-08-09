import React from 'react';
import type { VacancyDto, CompanyDto } from '@/types';
import { Briefcase, Building2, Calendar, ExternalLink, Edit3, ChevronRight } from 'lucide-react';

interface VacancyCardProps {
  vacancy: VacancyDto;
  company?: CompanyDto;
  onView: (vacancy: VacancyDto) => void;
  onEdit: (vacancy: VacancyDto) => void;
}

export const VacancyCard: React.FC<VacancyCardProps> = ({
  vacancy,
  company,
  onView,
  onEdit,
}) => {
  return (
    <div className="group relative flex flex-col justify-between rounded-xl border border-border/60 bg-card p-5 shadow-sm transition-all hover:border-primary/40 hover:shadow-md">
      <div>
        {/* Header: Title & Company */}
        <div className="flex items-start justify-between gap-3">
          <div className="flex items-start gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary font-bold mt-0.5">
              <Briefcase className="h-5 w-5" />
            </div>
            <div>
              <h3
                onClick={() => onView(vacancy)}
                className="font-bold text-base text-foreground tracking-tight hover:text-primary transition-colors cursor-pointer"
              >
                {vacancy.title}
              </h3>
              {company && (
                <div className="flex items-center gap-1 text-xs font-medium text-muted-foreground mt-0.5">
                  <Building2 className="h-3 w-3 text-primary/70" />
                  <span>{company.name}</span>
                </div>
              )}
            </div>
          </div>

          {vacancy.url && (
            <a
              href={vacancy.url}
              target="_blank"
              rel="noopener noreferrer"
              className="p-1.5 rounded-lg text-muted-foreground hover:text-primary hover:bg-accent transition-colors"
              title="Open listing"
              onClick={(e) => e.stopPropagation()}
            >
              <ExternalLink className="h-4 w-4" />
            </a>
          )}
        </div>

        {/* Posted date & Description snippet */}
        <div className="mt-3 space-y-2 text-xs">
          {vacancy.postedAt && (
            <div className="flex items-center gap-1.5 text-muted-foreground">
              <Calendar className="h-3.5 w-3.5 shrink-0 text-primary/80" />
              <span>Posted {new Date(vacancy.postedAt).toLocaleDateString()}</span>
            </div>
          )}

          {vacancy.description ? (
            <p className="line-clamp-3 text-muted-foreground text-xs leading-relaxed pt-1">
              {vacancy.description}
            </p>
          ) : (
            <p className="text-xs text-muted-foreground/40 italic pt-1">No description provided</p>
          )}
        </div>
      </div>

      {/* Footer bar */}
      <div className="mt-5 flex items-center justify-between border-t border-border/40 pt-3">
        <span className="text-[11px] text-muted-foreground/70">
          Added {new Date(vacancy.createdAt).toLocaleDateString()}
        </span>

        <div className="flex items-center gap-1">
          <button
            type="button"
            onClick={() => onEdit(vacancy)}
            className="rounded-lg p-1.5 text-muted-foreground hover:bg-accent hover:text-primary transition-colors"
            title="Edit Vacancy"
          >
            <Edit3 className="h-4 w-4" />
          </button>
          <button
            type="button"
            onClick={() => onView(vacancy)}
            className="inline-flex items-center gap-1 rounded-lg bg-accent/60 px-2.5 py-1 text-xs font-semibold text-foreground hover:bg-accent transition-colors"
          >
            Details <ChevronRight className="h-3.5 w-3.5" />
          </button>
        </div>
      </div>
    </div>
  );
};

export default VacancyCard;
