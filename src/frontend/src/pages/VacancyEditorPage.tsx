import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, useSearchParams, Link } from 'react-router-dom';
import { useCreateVacancy, useUpdateVacancy, useVacancy } from '@/api/vacancies';
import { useCompanies, useCompany } from '@/api/companies';
import CustomSelect from '@/components/ui/CustomSelect';
import ErrorAlert from '@/components/ui/ErrorAlert';
import type { WorkMode, SalaryCurrency, CreateVacancyDto, UpdateVacancyDto } from '@/types';
import {
  ArrowLeft,
  Briefcase,
  Building2,
  MapPin,
  DollarSign,
  Calendar,
  ExternalLink,
  FileText,
  Save,
  AlertCircle,
} from 'lucide-react';

const WORK_MODE_OPTIONS = [
  { value: '', label: 'Unspecified / Flexible' },
  { value: 'Remote', label: 'Remote' },
  { value: 'Hybride', label: 'Hybrid' },
  { value: 'Office', label: 'Office' },
];

const CURRENCY_OPTIONS = [
  { value: '', label: 'None' },
  { value: 'USD', label: 'USD ($)' },
  { value: 'EUR', label: 'EUR (€)' },
  { value: 'UAH', label: 'UAH (₴)' },
  { value: 'PLN', label: 'PLN (zł)' },
];

export const VacancyEditorPage: React.FC = () => {
  const { vacancyId } = useParams<{ vacancyId: string }>();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const isEditing = Boolean(vacancyId && vacancyId !== 'new');
  const queryCompanyId = searchParams.get('companyId') ?? '';

  // API hooks
  const {
    data: existingVacancy,
    isLoading: isLoadingVacancy,
    isError: isErrorVacancy,
  } = useVacancy(isEditing ? vacancyId : undefined);

  const { data: companies = [], isLoading: isLoadingCompanies } = useCompanies(false);
  const { data: existingCompany } = useCompany(isEditing ? existingVacancy?.companyId : undefined);

  const createMutation = useCreateVacancy();
  const updateMutation = useUpdateVacancy();
  const activeMutation = isEditing ? updateMutation : createMutation;

  // Form state
  const [companyId, setCompanyId] = useState('');
  const [title, setTitle] = useState('');
  const [url, setUrl] = useState('');
  const [location, setLocation] = useState('');
  const [workMode, setWorkMode] = useState<string>('');
  const [salaryMin, setSalaryMin] = useState<string>('');
  const [salaryMax, setSalaryMax] = useState<string>('');
  const [salaryCurrency, setSalaryCurrency] = useState<string>('USD');
  const [postedAt, setPostedAt] = useState('');
  const [description, setDescription] = useState('');
  const [validationError, setValidationError] = useState<string | null>(null);

  // Populate data when editing or creating
  useEffect(() => {
    if (isEditing && existingVacancy) {
      setCompanyId(existingVacancy.companyId);
      setTitle(existingVacancy.title || '');
      setUrl(existingVacancy.url || '');
      setLocation(existingVacancy.location || '');
      setWorkMode(existingVacancy.workMode || '');
      setSalaryMin(existingVacancy.salaryMin != null ? String(existingVacancy.salaryMin) : '');
      setSalaryMax(existingVacancy.salaryMax != null ? String(existingVacancy.salaryMax) : '');
      setSalaryCurrency(
        existingVacancy.salaryCurrency ||
          (existingVacancy.salaryMin != null || existingVacancy.salaryMax != null ? 'USD' : '')
      );
      setPostedAt(
        existingVacancy.postedAt
          ? new Date(existingVacancy.postedAt).toISOString().substring(0, 10)
          : ''
      );
      setDescription(existingVacancy.description || '');
    } else if (!isEditing) {
      const defaultCompany = queryCompanyId || (companies.length > 0 ? companies[0].id : '');
      setCompanyId(defaultCompany);
      setPostedAt(new Date().toISOString().substring(0, 10));
    }
  }, [isEditing, existingVacancy, queryCompanyId, companies]);

  // Handle auto-selection of company when companies finish loading in create mode
  useEffect(() => {
    if (!isEditing && !companyId && companies.length > 0) {
      setCompanyId(queryCompanyId || companies[0].id);
    }
  }, [companies, companyId, isEditing, queryCompanyId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);

    const trimmedTitle = title.trim();
    if (!trimmedTitle) {
      setValidationError('Vacancy title is required.');
      return;
    }

    if (!isEditing && !companyId) {
      setValidationError('Please select a company for this vacancy.');
      return;
    }

    const minNum = salaryMin !== '' ? Number(salaryMin) : null;
    const maxNum = salaryMax !== '' ? Number(salaryMax) : null;

    if (minNum != null && minNum < 0) {
      setValidationError('Minimum salary must not be negative.');
      return;
    }

    if (maxNum != null && maxNum > 1000000) {
      setValidationError('Maximum salary must not exceed 1,000,000.');
      return;
    }

    if (minNum != null && maxNum != null && minNum > maxNum) {
      setValidationError('Minimum salary cannot exceed maximum salary.');
      return;
    }

    try {
      if (isEditing && vacancyId) {
        const dto: UpdateVacancyDto = {
          title: trimmedTitle,
          description: description.trim() || null,
          url: url.trim() || null,
          location: location.trim() || null,
          workMode: (workMode as WorkMode) || null,
          salaryMin: minNum,
          salaryMax: maxNum,
          salaryCurrency: (salaryCurrency as SalaryCurrency) || null,
        };
        await updateMutation.mutateAsync({ id: vacancyId, dto });
        navigate(`/vacancies/${vacancyId}`);
      } else {
        const dto: CreateVacancyDto = {
          companyId,
          title: trimmedTitle,
          description: description.trim() || null,
          url: url.trim() || null,
          location: location.trim() || null,
          workMode: (workMode as WorkMode) || null,
          salaryMin: minNum,
          salaryMax: maxNum,
          salaryCurrency: (salaryCurrency as SalaryCurrency) || null,
          postedAt: postedAt ? new Date(postedAt).toISOString() : null,
        };
        const created = await createMutation.mutateAsync(dto);
        navigate(`/vacancies/${created.id}`);
      }
    } catch {
      // Error handled by activeMutation.error
    }
  };

  const backUrl = isEditing ? `/vacancies/${vacancyId}` : '/vacancies';

  if (isEditing && isLoadingVacancy) {
    return (
      <div className="space-y-6 max-w-4xl mx-auto py-6">
        <div className="h-8 w-32 bg-accent/60 rounded animate-pulse" />
        <div className="h-48 rounded-xl border border-border/60 bg-card p-6 animate-pulse space-y-4">
          <div className="h-6 w-1/3 bg-accent/60 rounded" />
          <div className="h-4 w-1/4 bg-accent/40 rounded" />
        </div>
      </div>
    );
  }

  if (isEditing && isErrorVacancy) {
    return (
      <div className="space-y-6 max-w-4xl mx-auto py-6">
        <Link
          to="/vacancies"
          className="inline-flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground"
        >
          <ArrowLeft className="h-4 w-4" /> Back to Vacancies
        </Link>
        <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-8 text-center">
          <AlertCircle className="mx-auto h-10 w-10 text-destructive mb-3" />
          <h3 className="text-base font-semibold text-destructive">Vacancy Not Found</h3>
          <p className="text-xs text-muted-foreground mt-1 mb-4">
            The vacancy you are trying to edit could not be loaded.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-4xl mx-auto pb-16">
      {/* Top Breadcrumb and Back Navigation */}
      <div className="flex items-center justify-between border-b border-border/40 pb-4">
        <Link
          to={backUrl}
          className="inline-flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors"
        >
          <ArrowLeft className="h-4 w-4" />
          {isEditing ? 'Back to Vacancy Details' : 'Back to Vacancies'}
        </Link>
      </div>

      {/* Page Header */}
      <div className="flex items-start gap-4">
        <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/10 text-primary font-bold">
          <Briefcase className="h-6 w-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-foreground">
            {isEditing ? 'Edit Vacancy' : 'Create Vacancy'}
          </h1>
          <p className="text-sm text-muted-foreground">
            {isEditing
              ? 'Update opportunity details, compensation package, and requirements.'
              : 'Add a new target opportunity and record key details for tailored applications.'}
          </p>
        </div>
      </div>

      <form onSubmit={handleSubmit} autoComplete="off" className="space-y-6">
        {/* Error Banners */}
        {validationError && (
          <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-4 text-xs font-semibold text-destructive flex items-center gap-2">
            <AlertCircle className="h-4 w-4 shrink-0" />
            <span>{validationError}</span>
          </div>
        )}
        {activeMutation.error && <ErrorAlert error={activeMutation.error} />}

        {/* Section 1: Basic Information */}
        <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
          <h2 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
            <Building2 className="h-4 w-4 text-primary" /> Basic Information
          </h2>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            {/* Target Company */}
            <div className="sm:col-span-2">
              <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                Target Company <span className="text-destructive">*</span>
              </label>
              {isEditing ? (
                <div className="flex items-center gap-2 rounded-lg border border-border/60 bg-accent/40 px-3.5 py-2.5 text-sm font-medium text-foreground">
                  <Building2 className="h-4 w-4 text-primary shrink-0" />
                  <span>{existingCompany?.name || 'Loading company...'}</span>
                  {existingCompany?.industry && (
                    <span className="text-xs text-muted-foreground">({existingCompany.industry})</span>
                  )}
                </div>
              ) : isLoadingCompanies ? (
                <div className="h-10 w-full rounded-lg bg-accent/40 animate-pulse" />
              ) : companies.length === 0 ? (
                <div className="rounded-lg border border-destructive/30 bg-destructive/10 p-3 text-xs text-destructive">
                  No companies found. Please create a company first before adding a vacancy.
                </div>
              ) : (
                <CustomSelect
                  value={companyId}
                  onChange={(val) => setCompanyId(val)}
                  placeholder="Select a target employer..."
                  options={companies.map((c) => ({
                    value: c.id,
                    label: `${c.name}${c.industry ? ` (${c.industry})` : ''}`,
                  }))}
                  className="w-full"
                />
              )}
            </div>

            {/* Vacancy Title */}
            <div className="sm:col-span-2">
              <label htmlFor="vacancy-title" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                Vacancy Title <span className="text-destructive">*</span>
              </label>
              <input
                id="vacancy-title"
                type="text"
                required
                maxLength={300}
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="e.g. Senior C# .NET Developer, Lead Backend Engineer"
                className="w-full rounded-lg border border-border bg-background px-3.5 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            {/* Original Listing URL */}
            <div>
              <label htmlFor="vacancy-url" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                Job Posting URL
              </label>
              <div className="relative">
                <ExternalLink className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground/60" />
                <input
                  id="vacancy-url"
                  type="url"
                  maxLength={1000}
                  value={url}
                  onChange={(e) => setUrl(e.target.value)}
                  placeholder="https://linkedin.com/jobs/view/..."
                  className="w-full rounded-lg border border-border bg-background pl-9 pr-3.5 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
                />
              </div>
            </div>

            {/* Posted Date (only on create per backend API DTO contract) */}
            {!isEditing ? (
              <div>
                <label htmlFor="vacancy-posted-at" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                  Posted Date
                </label>
                <div className="relative">
                  <Calendar className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground/60" />
                  <input
                    id="vacancy-posted-at"
                    type="date"
                    value={postedAt}
                    onChange={(e) => setPostedAt(e.target.value)}
                    className="w-full rounded-lg border border-border bg-background pl-9 pr-3.5 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
                  />
                </div>
              </div>
            ) : (
              <div className="flex flex-col justify-center text-xs text-muted-foreground">
                <span className="font-semibold uppercase tracking-wider text-[11px] mb-1">Posted Date</span>
                <span>
                  {existingVacancy?.postedAt
                    ? new Date(existingVacancy.postedAt).toLocaleDateString()
                    : 'Not specified'}
                </span>
              </div>
            )}
          </div>
        </div>

        {/* Section 2: Position Details & Compensation */}
        <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
          <h2 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
            <DollarSign className="h-4 w-4 text-primary" /> Role Details & Compensation
          </h2>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            {/* Location */}
            <div>
              <label htmlFor="vacancy-location" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                Location
              </label>
              <div className="relative">
                <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground/60" />
                <input
                  id="vacancy-location"
                  type="text"
                  maxLength={300}
                  value={location}
                  onChange={(e) => setLocation(e.target.value)}
                  placeholder="e.g. Kyiv, Ukraine or Remote / Europe"
                  className="w-full rounded-lg border border-border bg-background pl-9 pr-3.5 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
                />
              </div>
            </div>

            {/* Work Mode */}
            <div>
              <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                Work Mode
              </label>
              <CustomSelect
                value={workMode}
                onChange={(val) => setWorkMode(val)}
                placeholder="Select work mode..."
                options={WORK_MODE_OPTIONS}
                className="w-full"
              />
            </div>

            {/* Compensation Row */}
            <div className="sm:col-span-2 pt-2 border-t border-border/30">
              <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-2">
                Salary Range & Currency
              </label>
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                {/* Currency */}
                <div>
                  <label className="block text-[11px] font-medium text-muted-foreground mb-1">
                    Currency
                  </label>
                  <CustomSelect
                    value={salaryCurrency}
                    onChange={(val) => setSalaryCurrency(val)}
                    placeholder="Currency..."
                    options={CURRENCY_OPTIONS}
                    className="w-full"
                  />
                </div>

                {/* Min Salary */}
                <div>
                  <label htmlFor="vacancy-salary-min" className="block text-[11px] font-medium text-muted-foreground mb-1">
                    Minimum Salary
                  </label>
                  <input
                    id="vacancy-salary-min"
                    type="number"
                    min={0}
                    max={1000000}
                    value={salaryMin}
                    onChange={(e) => setSalaryMin(e.target.value)}
                    placeholder="e.g. 3500"
                    className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
                  />
                </div>

                {/* Max Salary */}
                <div>
                  <label htmlFor="vacancy-salary-max" className="block text-[11px] font-medium text-muted-foreground mb-1">
                    Maximum Salary
                  </label>
                  <input
                    id="vacancy-salary-max"
                    type="number"
                    min={0}
                    max={1000000}
                    value={salaryMax}
                    onChange={(e) => setSalaryMax(e.target.value)}
                    placeholder="e.g. 5000"
                    className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
                  />
                </div>
              </div>
              <p className="mt-1.5 text-[11px] text-muted-foreground/70">
                Optional: specify monthly/annual compensation or budget bounds for this opening.
              </p>
            </div>
          </div>
        </div>

        {/* Section 3: Job Description & Requirements */}
        <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
          <h2 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
            <FileText className="h-4 w-4 text-primary" /> Job Description & Requirements
          </h2>

          <div>
            <textarea
              id="vacancy-description"
              rows={8}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Paste job description, technical requirements, responsibilities, nice-to-haves..."
              className="w-full rounded-lg border border-border bg-background px-3.5 py-3 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary leading-relaxed resize-y"
            />
          </div>
        </div>

        {/* Bottom Form Actions */}
        <div className="flex items-center justify-between pt-4 border-t border-border/40">
          <button
            type="button"
            onClick={() => navigate(backUrl)}
            disabled={activeMutation.isPending}
            className="rounded-xl px-5 py-2.5 text-sm font-semibold text-muted-foreground hover:bg-accent hover:text-foreground transition-colors disabled:opacity-50 cursor-pointer"
          >
            Cancel
          </button>

          <button
            type="submit"
            disabled={activeMutation.isPending || !title.trim() || (!isEditing && !companyId)}
            className="inline-flex items-center gap-2 rounded-xl bg-primary px-6 py-2.5 text-sm font-bold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all disabled:opacity-50 cursor-pointer"
          >
            {activeMutation.isPending ? (
              isEditing ? 'Saving Changes...' : 'Creating Vacancy...'
            ) : (
              <>
                <Save className="h-4 w-4" />
                {isEditing ? 'Save Changes' : 'Create Vacancy'}
              </>
            )}
          </button>
        </div>
      </form>
    </div>
  );
};

export default VacancyEditorPage;
