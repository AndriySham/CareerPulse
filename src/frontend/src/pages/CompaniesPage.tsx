import React, { useState, useMemo } from 'react';
import { useCompanies } from '@/api/companies';
import CompanyCard from '@/components/companies/CompanyCard';
import CompanyFormModal from '@/components/companies/CompanyFormModal';
import CompanyDetailModal from '@/components/companies/CompanyDetailModal';
import VacancyFormModal from '@/components/vacancies/VacancyFormModal';
import VacancyDetailModal from '@/components/vacancies/VacancyDetailModal';
import { useVacancies } from '@/api/vacancies';
import type { CompanyDto, VacancyDto } from '@/types';
import { Building2, Search, Plus, Archive, CheckCircle2, SlidersHorizontal } from 'lucide-react';

export const CompaniesPage: React.FC = () => {
  const [includeArchived, setIncludeArchived] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');

  // Modal states
  const [isCompanyFormOpen, setIsCompanyFormOpen] = useState(false);
  const [editingCompany, setEditingCompany] = useState<CompanyDto | null>(null);

  const [isCompanyDetailOpen, setIsCompanyDetailOpen] = useState(false);
  const [viewingCompany, setViewingCompany] = useState<CompanyDto | null>(null);

  const [isVacancyFormOpen, setIsVacancyFormOpen] = useState(false);
  const [vacancyCompanyId, setVacancyCompanyId] = useState<string | undefined>(undefined);
  const [editingVacancy, setEditingVacancy] = useState<VacancyDto | null>(null);

  const [isVacancyDetailOpen, setIsVacancyDetailOpen] = useState(false);
  const [viewingVacancy, setViewingVacancy] = useState<VacancyDto | null>(null);

  // Queries
  const { data: companies = [], isLoading, isError, refetch } = useCompanies(includeArchived);
  const { data: vacancies = [] } = useVacancies();

  // Statistics
  const totalCompanies = companies.length;
  const activeCompanies = useMemo(() => companies.filter((c) => !c.isArchived).length, [companies]);
  const archivedCompanies = useMemo(() => companies.filter((c) => c.isArchived).length, [companies]);

  // Filtered companies based on search
  const filteredCompanies = useMemo(() => {
    if (!searchQuery.trim()) return companies;
    const query = searchQuery.toLowerCase();
    return companies.filter(
      (c) =>
        c.name.toLowerCase().includes(query) ||
        (c.industry && c.industry.toLowerCase().includes(query)) ||
        (c.notes && c.notes.toLowerCase().includes(query))
    );
  }, [companies, searchQuery]);

  // Modal actions
  const handleCreateCompany = () => {
    setEditingCompany(null);
    setIsCompanyFormOpen(true);
  };

  const handleEditCompany = (company: CompanyDto) => {
    setEditingCompany(company);
    setIsCompanyFormOpen(true);
  };

  const handleViewCompany = (company: CompanyDto) => {
    setViewingCompany(company);
    setIsCompanyDetailOpen(true);
  };

  const handleAddVacancy = (companyId: string) => {
    setEditingVacancy(null);
    setVacancyCompanyId(companyId);
    setIsVacancyFormOpen(true);
  };

  const handleSelectVacancyFromDetail = (vacancyId: string) => {
    const found = vacancies.find((v) => v.id === vacancyId);
    if (found) {
      setViewingVacancy(found);
      setIsVacancyDetailOpen(true);
    }
  };

  return (
    <div className="space-y-6">
      {/* Header section */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between border-b border-border/40 pb-5">
        <div className="flex items-center gap-3">
          <div className="p-2.5 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <Building2 className="h-6 w-6" />
          </div>
          <div>
            <h1 className="text-2xl font-bold tracking-tight text-foreground">Companies</h1>
            <p className="text-sm text-muted-foreground">
              Manage target employers and historical company contacts.
            </p>
          </div>
        </div>

        <button
          type="button"
          onClick={handleCreateCompany}
          className="inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2.5 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
        >
          <Plus className="h-4 w-4" /> Add Company
        </button>
      </div>

      {/* Summary KPI Cards */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-primary/10 text-primary">
            <Building2 className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{totalCompanies}</div>
            <div className="text-xs text-muted-foreground font-medium">Total Companies</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-emerald-500/10 text-emerald-500">
            <CheckCircle2 className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{activeCompanies}</div>
            <div className="text-xs text-muted-foreground font-medium">Active Companies</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-muted text-muted-foreground">
            <Archive className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{archivedCompanies}</div>
            <div className="text-xs text-muted-foreground font-medium">Archived Companies</div>
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
            placeholder="Search companies by name, industry, or notes..."
            className="w-full rounded-lg border border-border bg-background pl-9 pr-4 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        <div className="flex items-center gap-4 text-sm">
          <label className="flex items-center gap-2 cursor-pointer text-xs font-semibold text-muted-foreground hover:text-foreground">
            <input
              type="checkbox"
              checked={includeArchived}
              onChange={(e) => setIncludeArchived(e.target.checked)}
              className="h-4 w-4 rounded border-border text-primary focus:ring-primary accent-primary"
            />
            <span className="flex items-center gap-1">
              <SlidersHorizontal className="h-3.5 w-3.5" /> Include Archived
            </span>
          </label>
        </div>
      </div>

      {/* Content Section */}
      {isLoading ? (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {[1, 2, 3, 4, 5, 6].map((i) => (
            <div key={i} className="h-44 rounded-xl border border-border/60 bg-card p-5 animate-pulse space-y-3">
              <div className="h-5 w-1/2 bg-accent/60 rounded" />
              <div className="h-4 w-1/3 bg-accent/40 rounded" />
              <div className="h-10 w-full bg-accent/30 rounded mt-4" />
            </div>
          ))}
        </div>
      ) : isError ? (
        <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-8 text-center">
          <p className="text-sm text-destructive font-medium mb-2">Failed to load companies directory.</p>
          <button
            type="button"
            onClick={() => refetch()}
            className="rounded-lg bg-destructive px-4 py-2 text-xs font-semibold text-destructive-foreground hover:bg-destructive/90"
          >
            Retry
          </button>
        </div>
      ) : filteredCompanies.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border/80 bg-card p-12 text-center shadow-sm">
          <Building2 className="mx-auto h-12 w-12 text-muted-foreground/40 mb-3" />
          <h3 className="text-base font-semibold text-foreground">No companies found</h3>
          <p className="mt-1 text-sm text-muted-foreground max-w-sm mx-auto">
            {searchQuery
              ? 'No companies matched your search query. Try adjusting your filters.'
              : 'Start building your target employer directory by adding your first company.'}
          </p>
          <button
            type="button"
            onClick={handleCreateCompany}
            className="mt-4 inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all"
          >
            <Plus className="h-4 w-4" /> Add First Company
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {filteredCompanies.map((company) => (
            <CompanyCard
              key={company.id}
              company={company}
              onView={handleViewCompany}
              onEdit={handleEditCompany}
              onAddVacancy={handleAddVacancy}
            />
          ))}
        </div>
      )}

      {/* Modals */}
      <CompanyFormModal
        isOpen={isCompanyFormOpen}
        onClose={() => setIsCompanyFormOpen(false)}
        companyToEdit={editingCompany}
      />

      <CompanyDetailModal
        isOpen={isCompanyDetailOpen}
        onClose={() => setIsCompanyDetailOpen(false)}
        company={viewingCompany}
        onEditCompany={handleEditCompany}
        onAddVacancy={handleAddVacancy}
        onSelectVacancy={handleSelectVacancyFromDetail}
      />

      <VacancyFormModal
        isOpen={isVacancyFormOpen}
        onClose={() => setIsVacancyFormOpen(false)}
        vacancyToEdit={editingVacancy}
        initialCompanyId={vacancyCompanyId}
      />

      <VacancyDetailModal
        isOpen={isVacancyDetailOpen}
        onClose={() => setIsVacancyDetailOpen(false)}
        vacancy={viewingVacancy}
        onEditVacancy={(v) => {
          setIsVacancyDetailOpen(false);
          setEditingVacancy(v);
          setIsVacancyFormOpen(true);
        }}
      />
    </div>
  );
};

export default CompaniesPage;
