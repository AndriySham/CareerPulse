import React, { useState } from 'react';
import {
  useAnalyticsData,
  type AnalyticsFilterState,
  type AnalyticsTimeRange,
} from '@/api/analytics';
import AnalyticsKpiGrid from '@/components/analytics/AnalyticsKpiGrid';
import ApplicationFunnelChart from '@/components/analytics/ApplicationFunnelChart';
import JobSourceEfficiency from '@/components/analytics/JobSourceEfficiency';
import ResumeTrackAnalytics from '@/components/analytics/ResumeTrackAnalytics';
import SkillCoverageMatrix from '@/components/analytics/SkillCoverageMatrix';
import CompanyVacancyAnalytics from '@/components/analytics/CompanyVacancyAnalytics';
import CustomSelect from '@/components/ui/CustomSelect';
import {
  BarChart3,
  SlidersHorizontal,
  RefreshCw,
  Kanban,
  FileUser,
  Share2,
  Award,
  Building2,
  Calendar,
  Sparkles,
} from 'lucide-react';

export const AnalyticsPage: React.FC = () => {
  const [filters, setFilters] = useState<AnalyticsFilterState>({
    timeRange: 'all',
    resumeRevisionId: '',
    jobSource: '',
  });

  const [activeTab, setActiveTab] = useState<
    'overview' | 'funnel' | 'sources' | 'resumes' | 'skills' | 'companies'
  >('overview');

  const {
    isLoading,
    isError,
    refetchApps,
    filteredApplications,
    availableJobSources,
    kpiData,
    funnelStages,
    jobSourceMetrics,
    resumeRevisionMetrics,
    skillCategoryMetrics,
    companyMetrics,
    resumeRevisions,
  } = useAnalyticsData(filters);

  const timeRangeOptions: { value: AnalyticsTimeRange; label: string }[] = [
    { value: 'all', label: 'All Time' },
    { value: '7d', label: 'Last 7 Days' },
    { value: '30d', label: 'Last 30 Days' },
    { value: '90d', label: 'Last 3 Months' },
    { value: 'ytd', label: 'Year to Date' },
  ];

  const resumeOptions = [
    { value: '', label: 'All Resumes & Versions' },
    ...resumeRevisions.map((r) => ({
      value: r.id,
      label: `v${r.version} - ${r.personalInfo?.fullName || 'Candidate'} (${r.status})`,
    })),
  ];

  const sourceOptions = [
    { value: '', label: 'All Job Sources' },
    ...availableJobSources.map((s) => ({ value: s, label: s })),
  ];

  const hasActiveFilters =
    filters.timeRange !== 'all' || Boolean(filters.resumeRevisionId) || Boolean(filters.jobSource);

  const handleResetFilters = () => {
    setFilters({
      timeRange: 'all',
      resumeRevisionId: '',
      jobSource: '',
    });
  };

  return (
    <div className="space-y-6">
      {/* Header section */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between border-b border-border/40 pb-5">
        <div className="flex items-center gap-3">
          <div className="p-2.5 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <BarChart3 className="h-6 w-6" />
          </div>
          <div>
            <h1 className="text-2xl font-bold tracking-tight text-foreground">
              Career & Job Search Analytics
            </h1>
            <p className="text-sm text-muted-foreground">
              Data-driven insights into your application conversion, job source ROI, and skill coverage.
            </p>
          </div>
        </div>

        <div className="flex items-center gap-2">
          {hasActiveFilters && (
            <button
              type="button"
              onClick={handleResetFilters}
              className="inline-flex items-center gap-1.5 rounded-xl bg-accent px-3.5 py-2 text-xs font-semibold text-foreground hover:bg-accent/80 transition-all cursor-pointer border border-border/60"
            >
              Reset Filters
            </button>
          )}
          <button
            type="button"
            onClick={() => refetchApps()}
            className="inline-flex items-center gap-2 rounded-xl bg-secondary px-3.5 py-2 text-xs font-semibold text-secondary-foreground shadow-xs hover:bg-secondary/80 transition-all cursor-pointer border border-border/60"
          >
            <RefreshCw className="h-3.5 w-3.5" /> Refresh Analytics
          </button>
        </div>
      </div>

      {/* Filter and Control Toolbar */}
      <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between rounded-xl border border-border/60 bg-card p-4 shadow-sm">
        <div className="flex items-center gap-2 text-xs text-muted-foreground font-semibold">
          <SlidersHorizontal className="h-4 w-4 text-primary" />
          <span>Dashboard Filters:</span>
          {hasActiveFilters && (
            <span className="bg-primary/10 text-primary font-bold px-2 py-0.5 rounded-full text-[11px]">
              Active Filtered
            </span>
          )}
        </div>

        <div className="flex flex-wrap items-center gap-3">
          {/* Time Range Filter */}
          <div className="flex items-center gap-1.5">
            <Calendar className="h-3.5 w-3.5 text-muted-foreground" />
            <CustomSelect
              value={filters.timeRange}
              onChange={(val) => setFilters((prev) => ({ ...prev, timeRange: val as AnalyticsTimeRange }))}
              options={timeRangeOptions}
              className="w-40"
            />
          </div>

          {/* Resume Version Filter */}
          <CustomSelect
            value={filters.resumeRevisionId}
            onChange={(val) => setFilters((prev) => ({ ...prev, resumeRevisionId: val }))}
            options={resumeOptions}
            className="w-56"
          />

          {/* Job Source Filter */}
          <CustomSelect
            value={filters.jobSource}
            onChange={(val) => setFilters((prev) => ({ ...prev, jobSource: val }))}
            options={sourceOptions}
            className="w-44"
          />
        </div>
      </div>

      {/* Section Navigation Tabs */}
      <div className="flex items-center gap-1 border-b border-border/60 overflow-x-auto pb-px scrollbar-none">
        <button
          type="button"
          onClick={() => setActiveTab('overview')}
          className={`flex items-center gap-2 px-4 py-2.5 text-xs font-semibold border-b-2 transition-all cursor-pointer whitespace-nowrap ${
            activeTab === 'overview'
              ? 'border-primary text-primary font-bold'
              : 'border-transparent text-muted-foreground hover:text-foreground hover:border-border'
          }`}
        >
          <Sparkles className="h-4 w-4" /> Overview & KPIs
        </button>

        <button
          type="button"
          onClick={() => setActiveTab('funnel')}
          className={`flex items-center gap-2 px-4 py-2.5 text-xs font-semibold border-b-2 transition-all cursor-pointer whitespace-nowrap ${
            activeTab === 'funnel'
              ? 'border-primary text-primary font-bold'
              : 'border-transparent text-muted-foreground hover:text-foreground hover:border-border'
          }`}
        >
          <Kanban className="h-4 w-4" /> Application Funnel
        </button>

        <button
          type="button"
          onClick={() => setActiveTab('sources')}
          className={`flex items-center gap-2 px-4 py-2.5 text-xs font-semibold border-b-2 transition-all cursor-pointer whitespace-nowrap ${
            activeTab === 'sources'
              ? 'border-primary text-primary font-bold'
              : 'border-transparent text-muted-foreground hover:text-foreground hover:border-border'
          }`}
        >
          <Share2 className="h-4 w-4" /> Job Source ROI
        </button>

        <button
          type="button"
          onClick={() => setActiveTab('resumes')}
          className={`flex items-center gap-2 px-4 py-2.5 text-xs font-semibold border-b-2 transition-all cursor-pointer whitespace-nowrap ${
            activeTab === 'resumes'
              ? 'border-primary text-primary font-bold'
              : 'border-transparent text-muted-foreground hover:text-foreground hover:border-border'
          }`}
        >
          <FileUser className="h-4 w-4" /> Resume Versions
        </button>

        <button
          type="button"
          onClick={() => setActiveTab('skills')}
          className={`flex items-center gap-2 px-4 py-2.5 text-xs font-semibold border-b-2 transition-all cursor-pointer whitespace-nowrap ${
            activeTab === 'skills'
              ? 'border-primary text-primary font-bold'
              : 'border-transparent text-muted-foreground hover:text-foreground hover:border-border'
          }`}
        >
          <Award className="h-4 w-4" /> Skill Matrix
        </button>

        <button
          type="button"
          onClick={() => setActiveTab('companies')}
          className={`flex items-center gap-2 px-4 py-2.5 text-xs font-semibold border-b-2 transition-all cursor-pointer whitespace-nowrap ${
            activeTab === 'companies'
              ? 'border-primary text-primary font-bold'
              : 'border-transparent text-muted-foreground hover:text-foreground hover:border-border'
          }`}
        >
          <Building2 className="h-4 w-4" /> Companies & Vacancies
        </button>
      </div>

      {/* Main Analytics Content */}
      {isLoading ? (
        <div className="h-64 rounded-xl border border-border/60 bg-card p-8 flex flex-col items-center justify-center animate-pulse gap-3">
          <BarChart3 className="h-10 w-10 text-muted-foreground/40" />
          <p className="text-sm text-muted-foreground font-medium">Calculating career metrics and application analytics...</p>
        </div>
      ) : isError ? (
        <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-8 text-center">
          <p className="text-sm text-destructive font-medium mb-2">Failed to load career analytics data.</p>
          <button
            type="button"
            onClick={() => refetchApps()}
            className="rounded-lg bg-destructive px-4 py-2 text-xs font-semibold text-destructive-foreground hover:bg-destructive/90"
          >
            Retry
          </button>
        </div>
      ) : filteredApplications.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border/80 bg-card p-12 text-center shadow-sm space-y-3">
          <BarChart3 className="mx-auto h-12 w-12 text-muted-foreground/40" />
          <h3 className="text-base font-semibold text-foreground">No applications found</h3>
          <p className="text-sm text-muted-foreground max-w-sm mx-auto">
            {hasActiveFilters
              ? 'No applications match your active filter selections. Reset filters to see your complete analytics.'
              : 'Submit job applications to populate your career pipeline, conversion funnel, and metrics.'}
          </p>
          {hasActiveFilters ? (
            <button
              type="button"
              onClick={handleResetFilters}
              className="mt-2 inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
            >
              Reset Filters
            </button>
          ) : null}
        </div>
      ) : (
        <div className="space-y-6">
          {/* Always Show Top KPI Grid when on Overview */}
          {(activeTab === 'overview' || activeTab === 'funnel') && (
            <AnalyticsKpiGrid kpi={kpiData} />
          )}

          {activeTab === 'overview' && (
            <div className="space-y-6">
              <ApplicationFunnelChart funnelStages={funnelStages} kpi={kpiData} />
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                <JobSourceEfficiency sources={jobSourceMetrics} />
                <ResumeTrackAnalytics revisions={resumeRevisionMetrics} />
              </div>
              <SkillCoverageMatrix categories={skillCategoryMetrics} />
              <CompanyVacancyAnalytics companies={companyMetrics} kpi={kpiData} />
            </div>
          )}

          {activeTab === 'funnel' && (
            <ApplicationFunnelChart funnelStages={funnelStages} kpi={kpiData} />
          )}

          {activeTab === 'sources' && (
            <JobSourceEfficiency sources={jobSourceMetrics} />
          )}

          {activeTab === 'resumes' && (
            <ResumeTrackAnalytics revisions={resumeRevisionMetrics} />
          )}

          {activeTab === 'skills' && (
            <SkillCoverageMatrix categories={skillCategoryMetrics} />
          )}

          {activeTab === 'companies' && (
            <CompanyVacancyAnalytics companies={companyMetrics} kpi={kpiData} />
          )}
        </div>
      )}
    </div>
  );
};

export default AnalyticsPage;
