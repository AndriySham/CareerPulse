import React from 'react';
import { Link } from 'react-router-dom';
import { formatSalary, formatWorkMode } from '@/types';
import type { VacancyDto, CompanyDto } from '@/types';
import {
  Briefcase,
  Building2,
  Calendar,
  ExternalLink,
  Edit3,
  ChevronRight,
  Send,
  MapPin,
  Laptop,
  Banknote,
} from 'lucide-react';

interface VacancyCardProps {
  vacancy: VacancyDto;
  company?: CompanyDto;
  onView?: (vacancy: VacancyDto) => void;
  onEdit: (vacancy: VacancyDto) => void;
  onApply?: (vacancy: VacancyDto) => void;
}

export const VacancyCard: React.FC<VacancyCardProps> = ({
  vacancy,
  company,
  onView,
  onEdit,
  onApply,
}) => {
  const salaryText = formatSalary(vacancy.salaryMin, vacancy.salaryMax, vacancy.salaryCurrency);
  const workModeText = formatWorkMode(vacancy.workMode);

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
              <Link
                to={`/vacancies/${vacancy.id}`}
                onClick={() => onView?.(vacancy)}
                className="font-bold text-base text-foreground tracking-tight hover:text-primary transition-colors cursor-pointer block"
              >
                {vacancy.title}
              </Link>
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

        {/* Compact Metadata Tags: Work Mode, Location & Salary */}
        {(workModeText || vacancy.location || salaryText) && (
          <div className="mt-3 flex flex-wrap items-center gap-1.5 text-[11px]">
            {workModeText && (
              <span className="inline-flex items-center gap-1 rounded bg-accent/60 px-2 py-0.5 font-medium text-foreground">
                <Laptop className="h-3 w-3 text-primary" />
                {workModeText}
              </span>
            )}
            {vacancy.location && (
              <span className="inline-flex items-center gap-1 rounded bg-accent/60 px-2 py-0.5 font-medium text-muted-foreground truncate max-w-[150px]">
                <MapPin className="h-3 w-3 text-primary shrink-0" />
                <span className="truncate">{vacancy.location}</span>
              </span>
            )}
            {salaryText && (
              <span className="inline-flex items-center gap-1 rounded bg-emerald-500/10 px-2 py-0.5 font-semibold text-emerald-400 border border-emerald-500/20">
                <Banknote className="h-3 w-3" />
                {salaryText}
              </span>
            )}
          </div>
        )}

        {/* Posted date & Description snippet */}
        <div className="mt-3 space-y-2 text-xs">
          {vacancy.postedAt && (
            <div className="flex items-center gap-1.5 text-muted-foreground">
              <Calendar className="h-3.5 w-3.5 shrink-0 text-primary/80" />
              <span>Posted {new Date(vacancy.postedAt).toLocaleDateString()}</span>
            </div>
          )}

          {vacancy.description ? (
            <p className="line-clamp-2 text-muted-foreground text-xs leading-relaxed pt-0.5">
              {vacancy.description}
            </p>
          ) : (
            <p className="text-xs text-muted-foreground/40 italic pt-0.5">No description provided</p>
          )}
        </div>
      </div>

      {/* Footer bar */}
      <div className="mt-5 flex items-center justify-between border-t border-border/40 pt-3">
        <span className="text-[11px] text-muted-foreground/70">
          Added {new Date(vacancy.createdAt).toLocaleDateString()}
        </span>

        <div className="flex items-center gap-1.5">
          <button
            type="button"
            onClick={(e) => {
              e.stopPropagation();
              onEdit(vacancy);
            }}
            className="rounded-lg p-1.5 text-muted-foreground hover:bg-accent hover:text-primary transition-colors cursor-pointer"
            title="Edit Vacancy"
          >
            <Edit3 className="h-4 w-4" />
          </button>
          {onApply && (
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation();
                onApply(vacancy);
              }}
              className="inline-flex items-center gap-1 rounded-lg bg-primary/10 px-2.5 py-1 text-xs font-semibold text-primary hover:bg-primary hover:text-primary-foreground transition-all cursor-pointer"
              title="Apply for Vacancy"
            >
              <Send className="h-3.5 w-3.5" /> Apply
            </button>
          )}
          <Link
            to={`/vacancies/${vacancy.id}`}
            onClick={() => onView?.(vacancy)}
            className="inline-flex items-center gap-1 rounded-lg bg-accent/60 px-2.5 py-1 text-xs font-semibold text-foreground hover:bg-accent hover:text-primary transition-colors"
          >
            Details <ChevronRight className="h-3.5 w-3.5" />
          </Link>
        </div>
      </div>
    </div>
  );
};

export default VacancyCard;
