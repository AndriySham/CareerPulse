import React, { useState, useMemo } from 'react';
import { useVacancies } from '@/api/vacancies';
import { useCompanies } from '@/api/companies';
import VacancyCard from '@/components/vacancies/VacancyCard';
import VacancyFormModal from '@/components/vacancies/VacancyFormModal';
import VacancyDetailModal from '@/components/vacancies/VacancyDetailModal';
import CustomSelect from '@/components/ui/CustomSelect';
import type { VacancyDto } from '@/types';
import { Briefcase, Search, Plus, Building2, Filter } from 'lucide-react';

export const VacanciesPage: React.FC = () => {
  const [selectedCompanyId, setSelectedCompanyId] = useState<string>('all');
  const [searchQuery, setSearchQuery] = useState('');

  // Modal states
  const [isVacancyFormOpen, setIsVacancyFormOpen] = useState(false);
  const [editingVacancy, setEditingVacancy] = useState<VacancyDto | null>(null);

  const [isVacancyDetailOpen, setIsVacancyDetailOpen] = useState(false);
  const [viewingVacancy, setViewingVacancy] = useState<VacancyDto | null>(null);

  // Queries
  const { data: companies = [] } = useCompanies(true);
  const companyMap = useMemo(() => {
    const map = new Map<string, (typeof companies)[0]>();
    companies.forEach((c) => map.set(c.id, c));
    return map;
  }, [companies]);

  const filterCompanyParam = selectedCompanyId === 'all' ? undefined : selectedCompanyId;
  const { data: vacancies = [], isLoading, isError, refetch } = useVacancies(filterCompanyParam);

  // Filtered vacancies based on search text
  const filteredVacancies = useMemo(() => {
    if (!searchQuery.trim()) return vacancies;
    const query = searchQuery.toLowerCase();
    return vacancies.filter((v) => {
      const company = companyMap.get(v.companyId);
      return (
        v.title.toLowerCase().includes(query) ||
        (v.description && v.description.toLowerCase().includes(query)) ||
        (company && company.name.toLowerCase().includes(query))
      );
    });
  }, [vacancies, searchQuery, companyMap]);

  // Statistics
  const totalVacancies = vacancies.length;
  const uniqueCompaniesWithVacancies = useMemo(() => {
    const set = new Set(vacancies.map((v) => v.companyId));
    return set.size;
  }, [vacancies]);

  // Actions
  const handleCreateVacancy = () => {
    setEditingVacancy(null);
    setIsVacancyFormOpen(true);
  };

  const handleEditVacancy = (vacancy: VacancyDto) => {
    setEditingVacancy(vacancy);
    setIsVacancyFormOpen(true);
  };

  const handleViewVacancy = (vacancy: VacancyDto) => {
    setViewingVacancy(vacancy);
    setIsVacancyDetailOpen(true);
  };

  return (
    <div className="space-y-6">
      {/* Header section */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between border-b border-border/40 pb-5">
        <div className="flex items-center gap-3">
          <div className="p-2.5 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <Briefcase className="h-6 w-6" />
          </div>
          <div>
            <h1 className="text-2xl font-bold tracking-tight text-foreground">Vacancies</h1>
            <p className="text-sm text-muted-foreground">
              View and track targeted job opportunities and vacancy requirements.
            </p>
          </div>
        </div>

        <button
          type="button"
          onClick={handleCreateVacancy}
          className="inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2.5 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
        >
          <Plus className="h-4 w-4" /> Add Vacancy
        </button>
      </div>

      {/* Summary KPI Cards */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-primary/10 text-primary">
            <Briefcase className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{totalVacancies}</div>
            <div className="text-xs text-muted-foreground font-medium">Total Target Vacancies</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-accent text-foreground">
            <Building2 className="h-5 w-5 text-primary" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{uniqueCompaniesWithVacancies}</div>
            <div className="text-xs text-muted-foreground font-medium">Companies with Openings</div>
          </div>
        </div>
      </div>

      {/* Filter and Search Bar */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between rounded-xl border border-border/60 bg-card p-4 shadow-sm">
        <div className="relative flex-1 max-w-md">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search vacancies by title, company, or tech stack..."
            className="w-full rounded-lg border border-border bg-background pl-9 pr-4 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        <div className="flex items-center gap-2 text-sm">
          <Filter className="h-4 w-4 text-muted-foreground shrink-0" />
          <CustomSelect
            value={selectedCompanyId}
            onChange={(val) => setSelectedCompanyId(val)}
            options={[
              { value: 'all', label: `All Employers (${companies.length})` },
              ...companies.map((c) => ({ value: c.id, label: c.name })),
            ]}
            className="w-48"
          />
        </div>
      </div>

      {/* Content Section */}
      {isLoading ? (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {[1, 2, 3, 4, 5, 6].map((i) => (
            <div key={i} className="h-48 rounded-xl border border-border/60 bg-card p-5 animate-pulse space-y-3">
              <div className="h-5 w-2/3 bg-accent/60 rounded" />
              <div className="h-4 w-1/3 bg-accent/40 rounded" />
              <div className="h-14 w-full bg-accent/30 rounded mt-4" />
            </div>
          ))}
        </div>
      ) : isError ? (
        <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-8 text-center">
          <p className="text-sm text-destructive font-medium mb-2">Failed to load vacancies.</p>
          <button
            type="button"
            onClick={() => refetch()}
            className="rounded-lg bg-destructive px-4 py-2 text-xs font-semibold text-destructive-foreground hover:bg-destructive/90"
          >
            Retry
          </button>
        </div>
      ) : filteredVacancies.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border/80 bg-card p-12 text-center shadow-sm">
          <Briefcase className="mx-auto h-12 w-12 text-muted-foreground/40 mb-3" />
          <h3 className="text-base font-semibold text-foreground">No vacancies found</h3>
          <p className="mt-1 text-sm text-muted-foreground max-w-sm mx-auto">
            {searchQuery || selectedCompanyId !== 'all'
              ? 'No vacancies matched your search or company filter.'
              : 'Add your first job vacancy to track job requirements and tailored applications.'}
          </p>
          <button
            type="button"
            onClick={handleCreateVacancy}
            className="mt-4 inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all"
          >
            <Plus className="h-4 w-4" /> Add First Vacancy
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {filteredVacancies.map((vacancy) => (
            <VacancyCard
              key={vacancy.id}
              vacancy={vacancy}
              company={companyMap.get(vacancy.companyId)}
              onView={handleViewVacancy}
              onEdit={handleEditVacancy}
            />
          ))}
        </div>
      )}

      {/* Modals */}
      <VacancyFormModal
        isOpen={isVacancyFormOpen}
        onClose={() => setIsVacancyFormOpen(false)}
        vacancyToEdit={editingVacancy}
        initialCompanyId={selectedCompanyId !== 'all' ? selectedCompanyId : undefined}
      />

      <VacancyDetailModal
        isOpen={isVacancyDetailOpen}
        onClose={() => setIsVacancyDetailOpen(false)}
        vacancy={viewingVacancy}
        onEditVacancy={handleEditVacancy}
      />
    </div>
  );
};

export default VacanciesPage;
