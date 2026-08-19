import React from 'react';
import type { CompanyMetric, AnalyticsKpiData } from '@/api/analytics';
import ApplicationStatusBadge from '@/components/applications/ApplicationStatusBadge';
import { Building2, Briefcase, Link2, CheckCircle2 } from 'lucide-react';

interface CompanyVacancyAnalyticsProps {
  companies: CompanyMetric[];
  kpi: AnalyticsKpiData;
}

export const CompanyVacancyAnalytics: React.FC<CompanyVacancyAnalyticsProps> = ({
  companies,
  kpi,
}) => {
  const topCompanies = companies.slice(0, 8);

  return (
    <div className="rounded-xl border border-border/60 bg-card p-5 shadow-sm space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between border-b border-border/40 pb-4 gap-2">
        <div className="flex items-center gap-2.5">
          <div className="p-2 rounded-lg bg-amber-500/10 text-amber-500">
            <Building2 className="h-5 w-5" />
          </div>
          <div>
            <h2 className="text-base font-bold text-foreground">Employer & Vacancy Distribution</h2>
            <p className="text-xs text-muted-foreground">
              Target company distribution and vacancy linkage rate across your career pipeline.
            </p>
          </div>
        </div>

        <div className="flex items-center gap-2 text-xs">
          <span className="inline-flex items-center gap-1 font-semibold text-amber-500 bg-amber-500/10 px-2.5 py-1 rounded-full border border-amber-500/20">
            <Link2 className="h-3.5 w-3.5" /> {kpi.vacancyLinkageRate}% Vacancy Linked
          </span>
        </div>
      </div>

      {companies.length === 0 ? (
        <div className="p-8 text-center text-sm text-muted-foreground">
          No company application metrics recorded yet.
        </div>
      ) : (
        <div className="space-y-4">
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-3">
            {topCompanies.map((c) => (
              <div
                key={c.companyId}
                className="rounded-xl border border-border/60 bg-accent/20 p-3.5 space-y-2 hover:border-amber-500/40 transition-all flex flex-col justify-between"
              >
                <div>
                  <div className="flex items-center justify-between">
                    <div className="font-bold text-sm text-foreground truncate max-w-[140px]" title={c.companyName}>
                      {c.companyName}
                    </div>
                    <span className="text-xs font-bold text-primary bg-primary/10 px-2 py-0.5 rounded-full">
                      {c.applicationsCount} app(s)
                    </span>
                  </div>

                  <div className="mt-2 flex items-center justify-between text-xs">
                    <span className="text-muted-foreground font-medium flex items-center gap-1">
                      <Briefcase className="h-3 w-3 text-muted-foreground" />
                      {c.hasActiveVacancy ? 'Vacancy linked' : 'General app'}
                    </span>
                  </div>
                </div>

                <div className="pt-2 border-t border-border/40 flex items-center justify-between">
                  <span className="text-[11px] text-muted-foreground font-semibold">Latest Status:</span>
                  <ApplicationStatusBadge status={c.latestStatus} size="sm" />
                </div>
              </div>
            ))}
          </div>

          {/* Company Coverage Summary Card */}
          <div className="rounded-xl border border-border/40 bg-accent/40 p-4 flex flex-col sm:flex-row items-center justify-between text-xs gap-3">
            <div className="flex items-center gap-3">
              <div className="p-2 rounded-lg bg-emerald-500/10 text-emerald-500">
                <CheckCircle2 className="h-5 w-5" />
              </div>
              <div>
                <div className="font-bold text-foreground">Target Company Footprint</div>
                <div className="text-muted-foreground">
                Applications distributed across <strong className="text-foreground">{companies.length}</strong> targeted companies.
              </div>
              </div>
            </div>

            <div className="text-right">
              <div className="font-bold text-base text-primary">{companies.length} Total CRM Companies</div>
              <div className="text-muted-foreground text-[11px]">Tracked in database</div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CompanyVacancyAnalytics;
