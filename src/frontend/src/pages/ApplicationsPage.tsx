import React, { useState, useMemo } from 'react';
import { useApplications, useChangeApplicationStatus } from '@/api/applications';
import { useCompanies } from '@/api/companies';
import { useVacancies } from '@/api/vacancies';
import KanbanBoard from '@/components/applications/KanbanBoard';
import ApplicationStatusBadge from '@/components/applications/ApplicationStatusBadge';
import ApplicationFormModal from '@/components/applications/ApplicationFormModal';
import ApplicationDetailModal from '@/components/applications/ApplicationDetailModal';
import CustomSelect from '@/components/ui/CustomSelect';
import type { ApplicationDto, ApplicationStatus } from '@/types';
import {
  Kanban,
  List,
  Search,
  Plus,
  Building2,
  Award,
  Send,
  SlidersHorizontal,
  RefreshCw,
} from 'lucide-react';

export const ApplicationsPage: React.FC = () => {
  const [viewMode, setViewMode] = useState<'kanban' | 'list'>('kanban');
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCompanyId, setSelectedCompanyId] = useState<string>('');
  const [selectedVacancyId, setSelectedVacancyId] = useState<string>('');

  // Modal states
  const [isSubmitModalOpen, setIsSubmitModalOpen] = useState(false);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);
  const [viewingApplication, setViewingApplication] = useState<ApplicationDto | null>(null);

  // Queries
  const {
    data: applications = [],
    isLoading,
    isError,
    refetch,
  } = useApplications({
    companyId: selectedCompanyId || undefined,
    vacancyId: selectedVacancyId || undefined,
  });

  const { data: companies = [] } = useCompanies();
  const { data: vacancies = [] } = useVacancies(selectedCompanyId || undefined);

  // Status Change Mutation
  const changeStatusMutation = useChangeApplicationStatus();

  // Statistics
  const totalApplications = applications.length;
  const activeApplications = useMemo(
    () =>
      applications.filter((a) =>
        ['Applied', 'Viewed', 'HRInterview', 'TechnicalInterview'].includes(a.status)
      ).length,
    [applications]
  );
  const offersCount = useMemo(
    () => applications.filter((a) => a.status === 'Offer').length,
    [applications]
  );
  const responseRate = useMemo(() => {
    if (totalApplications === 0) return '0%';
    const responded = applications.filter((a) => a.status !== 'Draft' && a.status !== 'Applied').length;
    return `${Math.round((responded / totalApplications) * 100)}%`;
  }, [applications, totalApplications]);

  // Filtered applications based on search query
  const filteredApplications = useMemo(() => {
    if (!searchQuery.trim()) return applications;
    const query = searchQuery.toLowerCase();
    return applications.filter(
      (a) =>
        a.companyName.toLowerCase().includes(query) ||
        (a.vacancyTitle && a.vacancyTitle.toLowerCase().includes(query)) ||
        a.jobSource.toLowerCase().includes(query) ||
        (a.notes && a.notes.toLowerCase().includes(query))
    );
  }, [applications, searchQuery]);

  const handleViewApplication = (app: ApplicationDto) => {
    setViewingApplication(app);
    setIsDetailModalOpen(true);
  };

  const handleChangeStatus = (app: ApplicationDto, newStatus: ApplicationStatus) => {
    changeStatusMutation.mutate(
      { id: app.id, dto: { newStatus } },
      {
        onSuccess: (updatedApp) => {
          if (viewingApplication?.id === updatedApp.id) {
            setViewingApplication(updatedApp);
          }
        },
      }
    );
  };

  return (
    <div className="space-y-6">
      {/* Header section */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between border-b border-border/40 pb-5">
        <div className="flex items-center gap-3">
          <div className="p-2.5 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <Kanban className="h-6 w-6" />
          </div>
          <div>
            <h1 className="text-2xl font-bold tracking-tight text-foreground">
              Applications & Kanban Pipeline
            </h1>
            <p className="text-sm text-muted-foreground">
              Track and manage your job applications through their lifecycle.
            </p>
          </div>
        </div>

        <button
          type="button"
          onClick={() => setIsSubmitModalOpen(true)}
          className="inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2.5 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
        >
          <Plus className="h-4 w-4" /> Submit Application
        </button>
      </div>

      {/* KPI Summary Cards */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-4">
        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-primary/10 text-primary">
            <Kanban className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{totalApplications}</div>
            <div className="text-xs text-muted-foreground font-medium">Total Applications</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-blue-500/10 text-blue-500">
            <Send className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{activeApplications}</div>
            <div className="text-xs text-muted-foreground font-medium">Active In Pipeline</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-emerald-500/10 text-emerald-500">
            <Award className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{offersCount}</div>
            <div className="text-xs text-muted-foreground font-medium">Offers Received</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-purple-500/10 text-purple-500">
            <RefreshCw className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{responseRate}</div>
            <div className="text-xs text-muted-foreground font-medium">Employer Response Rate</div>
          </div>
        </div>
      </div>

      {/* Filter and Control Toolbar */}
      <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between rounded-xl border border-border/60 bg-card p-4 shadow-sm">
        {/* Search */}
        <div className="relative flex-1 max-w-md">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search by company, vacancy, source, or notes..."
            className="w-full rounded-lg border border-border bg-background pl-9 pr-4 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        {/* Company & Vacancy Select Filters */}
        <div className="flex flex-wrap items-center gap-3">
          <div className="flex items-center gap-1.5 text-xs text-muted-foreground font-medium">
            <SlidersHorizontal className="h-3.5 w-3.5" />
            <span>Filters:</span>
          </div>

          <CustomSelect
            value={selectedCompanyId}
            onChange={(val) => {
              setSelectedCompanyId(val);
              setSelectedVacancyId('');
            }}
            options={[
              { value: '', label: 'All Companies' },
              ...companies.map((c) => ({ value: c.id, label: c.name })),
            ]}
            className="w-44"
          />

          <CustomSelect
            value={selectedVacancyId}
            onChange={(val) => setSelectedVacancyId(val)}
            options={[
              { value: '', label: 'All Vacancies' },
              ...vacancies.map((v) => ({ value: v.id, label: v.title })),
            ]}
            className="w-44"
          />

          {/* View mode toggle */}
          <div className="flex items-center rounded-lg border border-border bg-background p-1 gap-1">
            <button
              type="button"
              onClick={() => setViewMode('kanban')}
              className={`flex items-center gap-1 px-2.5 py-1 text-xs font-semibold rounded-md transition-colors ${
                viewMode === 'kanban'
                  ? 'bg-primary text-primary-foreground shadow-sm'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
            >
              <Kanban className="h-3.5 w-3.5" /> Kanban
            </button>
            <button
              type="button"
              onClick={() => setViewMode('list')}
              className={`flex items-center gap-1 px-2.5 py-1 text-xs font-semibold rounded-md transition-colors ${
                viewMode === 'list'
                  ? 'bg-primary text-primary-foreground shadow-sm'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
            >
              <List className="h-3.5 w-3.5" /> List
            </button>
          </div>
        </div>
      </div>

      {/* Main Content Area */}
      {isLoading ? (
        <div className="h-64 rounded-xl border border-border/60 bg-card p-8 flex items-center justify-center animate-pulse">
          <p className="text-sm text-muted-foreground">Loading applications board...</p>
        </div>
      ) : isError ? (
        <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-8 text-center">
          <p className="text-sm text-destructive font-medium mb-2">Failed to load applications.</p>
          <button
            type="button"
            onClick={() => refetch()}
            className="rounded-lg bg-destructive px-4 py-2 text-xs font-semibold text-destructive-foreground hover:bg-destructive/90"
          >
            Retry
          </button>
        </div>
      ) : filteredApplications.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border/80 bg-card p-12 text-center shadow-sm">
          <Kanban className="mx-auto h-12 w-12 text-muted-foreground/40 mb-3" />
          <h3 className="text-base font-semibold text-foreground">No applications found</h3>
          <p className="mt-1 text-sm text-muted-foreground max-w-sm mx-auto">
            {searchQuery || selectedCompanyId || selectedVacancyId
              ? 'No applications match your active filters or search terms.'
              : 'Submit your first job application to start tracking your search pipeline.'}
          </p>
          <button
            type="button"
            onClick={() => setIsSubmitModalOpen(true)}
            className="mt-4 inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all"
          >
            <Plus className="h-4 w-4" /> Submit Application
          </button>
        </div>
      ) : viewMode === 'kanban' ? (
        <KanbanBoard
          applications={filteredApplications}
          onViewApplication={handleViewApplication}
          onChangeStatus={handleChangeStatus}
        />
      ) : (
        /* Table List View */
        <div className="rounded-xl border border-border/60 bg-card shadow-sm overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead className="border-b border-border/60 bg-accent/40 text-muted-foreground font-semibold">
                <tr>
                  <th className="px-4 py-3">Company</th>
                  <th className="px-4 py-3">Vacancy</th>
                  <th className="px-4 py-3">Status</th>
                  <th className="px-4 py-3">Source</th>
                  <th className="px-4 py-3">Date</th>
                  <th className="px-4 py-3 text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border/40">
                {filteredApplications.map((app) => (
                  <tr
                    key={app.id}
                    className="hover:bg-accent/20 transition-colors cursor-pointer"
                    onClick={() => handleViewApplication(app)}
                  >
                    <td className="px-4 py-3 font-bold text-foreground">
                      <div className="flex items-center gap-2">
                        <Building2 className="h-4 w-4 text-primary shrink-0" />
                        {app.companyName}
                      </div>
                    </td>
                    <td className="px-4 py-3 text-muted-foreground">
                      {app.vacancyTitle ?? <span className="italic opacity-60">General</span>}
                    </td>
                    <td className="px-4 py-3">
                      <ApplicationStatusBadge status={app.status} size="sm" />
                    </td>
                    <td className="px-4 py-3 text-muted-foreground">{app.jobSource}</td>
                    <td className="px-4 py-3 text-muted-foreground">
                      {app.appliedAt
                        ? new Date(app.appliedAt).toLocaleDateString()
                        : new Date(app.createdAt).toLocaleDateString()}
                    </td>
                    <td className="px-4 py-3 text-right">
                      <button
                        type="button"
                        onClick={(e) => {
                          e.stopPropagation();
                          handleViewApplication(app);
                        }}
                        className="rounded-lg bg-accent px-2.5 py-1 text-xs font-semibold text-foreground hover:bg-accent/80 transition-colors"
                      >
                        Details
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Modals */}
      <ApplicationFormModal
        isOpen={isSubmitModalOpen}
        onClose={() => setIsSubmitModalOpen(false)}
        initialCompanyId={selectedCompanyId}
        initialVacancyId={selectedVacancyId}
      />

      <ApplicationDetailModal
        isOpen={isDetailModalOpen}
        onClose={() => setIsDetailModalOpen(false)}
        application={viewingApplication}
      />
    </div>
  );
};

export default ApplicationsPage;
