import { useMemo } from 'react';
import { useApplications } from './applications';
import { useResumeRevisions } from './resumes';
import { useCompanies } from './companies';
import { useVacancies } from './vacancies';
import type {
  ApplicationStatus,
  SkillCategory,
} from '@/types';

export type AnalyticsTimeRange = 'all' | '7d' | '30d' | '90d' | 'ytd';

export interface AnalyticsFilterState {
  timeRange: AnalyticsTimeRange;
  resumeRevisionId: string;
  jobSource: string;
}

export interface AnalyticsKpiData {
  totalApplications: number;
  draftApplications: number;
  submittedApplications: number;
  activeApplications: number;
  interviewCount: number;
  hrInterviewCount: number;
  techInterviewCount: number;
  offerCount: number;
  rejectedCount: number;
  noResponseCount: number;
  ghostingRate: number; // percentage of submitted applications that received no response
  rejectionRate: number; // percentage of submitted applications that were rejected
  interviewConversionRate: number; // percentage
  techRoundRate: number; // percentage
  offerConversionRate: number; // percentage
  responseRate: number; // percentage (strictly Viewed, HRInterview, TechnicalInterview, Offer, Rejected / submitted)
  vacancyLinkageRate: number; // percentage
}

export interface FunnelStageData {
  status: ApplicationStatus;
  label: string;
  count: number;
  percentageOfSubmitted: number;
  conversionFromPrevious: number;
  color: string;
}

export interface JobSourceMetric {
  sourceName: string;
  totalApplications: number;
  submittedCount: number;
  activeCount: number;
  respondedCount: number;
  responseRate: number;
  noResponseCount: number;
  noResponseRate: number;
  hrInterviewCount: number;
  hrInterviewRate: number;
  techInterviewCount: number;
  techInterviewRate: number;
  offerCount: number;
  offerRate: number;
  rejectedCount: number;
  rejectionRate: number;
  shareOfTotal: number;
}

export interface ResumeRevisionMetric {
  revisionId: string;
  version: number;
  status: string;
  fullName: string;
  skillsCount: number;
  applicationsCount: number;
  submittedCount: number;
  respondedCount: number;
  responseRate: number;
  noResponseCount: number;
  noResponseRate: number;
  hrInterviewCount: number;
  hrInterviewRate: number;
  techInterviewCount: number;
  techInterviewRate: number;
  offersCount: number;
  offerRate: number;
  rejectionsCount: number;
  rejectionRate: number;
}

export interface SkillItemMetric {
  name: string;
  proficiency: number;
  count: number; // frequency of appearance across revisions
  submittedAppCount: number; // applications using this skill
  interviewAppCount: number; // applications using this skill that reached interview
  offerAppCount: number; // applications using this skill that reached offer
}

export interface SkillCategoryMetric {
  category: SkillCategory;
  categoryLabel: string;
  skillCount: number;
  averageProficiency: number;
  skills: SkillItemMetric[];
}

export interface CompanyMetric {
  companyId: string;
  companyName: string;
  applicationsCount: number;
  latestStatus: ApplicationStatus;
  hasActiveVacancy: boolean;
}

export function useAnalyticsData(filters: AnalyticsFilterState) {
  const {
    data: applications = [],
    isLoading: isLoadingApps,
    isError: isErrorApps,
    refetch: refetchApps,
  } = useApplications();

  const {
    data: resumeRevisions = [],
    isLoading: isLoadingResumes,
    isError: isErrorResumes,
  } = useResumeRevisions();

  const {
    data: companies = [],
    isLoading: isLoadingCompanies,
    isError: isErrorCompanies,
  } = useCompanies();

  const {
    data: vacancies = [],
    isLoading: isLoadingVacancies,
    isError: isErrorVacancies,
  } = useVacancies();

  const isLoading =
    isLoadingApps || isLoadingResumes || isLoadingCompanies || isLoadingVacancies;
  const isError =
    isErrorApps || isErrorResumes || isErrorCompanies || isErrorVacancies;

  // Filtered applications based on timeRange, resumeRevisionId, jobSource
  const filteredApplications = useMemo(() => {
    const now = new Date();
    return applications.filter((app) => {
      // 1. Time Range Filter
      if (filters.timeRange !== 'all') {
        const appDate = new Date(app.appliedAt || app.createdAt);
        let daysLimit = 365;
        if (filters.timeRange === '7d') daysLimit = 7;
        else if (filters.timeRange === '30d') daysLimit = 30;
        else if (filters.timeRange === '90d') daysLimit = 90;
        else if (filters.timeRange === 'ytd') {
          const startOfYear = new Date(now.getFullYear(), 0, 1);
          if (appDate < startOfYear) return false;
          daysLimit = 0;
        }

        if (daysLimit > 0) {
          const diffMs = now.getTime() - appDate.getTime();
          const diffDays = diffMs / (1000 * 60 * 60 * 24);
          // Ignore items older than limit or unreasonable future dates (> 1 day skew)
          if (diffDays > daysLimit || diffDays < -1) return false;
        }
      }

      // 2. Resume Revision Filter
      if (filters.resumeRevisionId && app.resumeRevisionId !== filters.resumeRevisionId) {
        return false;
      }

      // 3. Job Source Filter
      if (filters.jobSource && app.jobSource.toLowerCase() !== filters.jobSource.toLowerCase()) {
        return false;
      }

      return true;
    });
  }, [applications, filters]);

  // Distinct Job Sources available in data dynamically
  const availableJobSources = useMemo(() => {
    const sourcesSet = new Set<string>();
    applications.forEach((a) => {
      if (a.jobSource) sourcesSet.add(a.jobSource);
    });
    return Array.from(sourcesSet).sort();
  }, [applications]);

  // KPI Calculations according to strict domain rules
  const kpiData = useMemo<AnalyticsKpiData>(() => {
    const total = filteredApplications.length;
    const drafts = filteredApplications.filter((a) => a.status === 'Draft').length;
    const submittedApps = filteredApplications.filter((a) => a.status !== 'Draft');
    const submittedCount = submittedApps.length;

    const active = submittedApps.filter((a) =>
      ['Applied', 'Viewed', 'HRInterview', 'TechnicalInterview'].includes(a.status)
    ).length;

    const hrInterviews = submittedApps.filter((a) =>
      ['HRInterview', 'TechnicalInterview', 'Offer'].includes(a.status)
    ).length;

    const techInterviews = submittedApps.filter((a) =>
      ['TechnicalInterview', 'Offer'].includes(a.status)
    ).length;

    const offers = submittedApps.filter((a) => a.status === 'Offer').length;
    const rejected = submittedApps.filter((a) => a.status === 'Rejected').length;
    const noResponse = submittedApps.filter((a) => a.status === 'NoResponse').length;

    // Domain Rule: Employer responses = Viewed, HRInterview, TechnicalInterview, Offer, Rejected.
    // Excludes Draft and NoResponse!
    const respondedApps = submittedApps.filter((a) =>
      ['Viewed', 'HRInterview', 'TechnicalInterview', 'Offer', 'Rejected'].includes(a.status)
    ).length;

    const linkedVacancies = filteredApplications.filter((a) => Boolean(a.vacancyId)).length;

    return {
      totalApplications: total,
      draftApplications: drafts,
      submittedApplications: submittedCount,
      activeApplications: active,
      interviewCount: hrInterviews,
      hrInterviewCount: hrInterviews,
      techInterviewCount: techInterviews,
      offerCount: offers,
      rejectedCount: rejected,
      noResponseCount: noResponse,
      ghostingRate: submittedCount > 0 ? Math.round((noResponse / submittedCount) * 100) : 0,
      rejectionRate: submittedCount > 0 ? Math.round((rejected / submittedCount) * 100) : 0,
      interviewConversionRate: submittedCount > 0 ? Math.round((hrInterviews / submittedCount) * 100) : 0,
      techRoundRate: submittedCount > 0 ? Math.round((techInterviews / submittedCount) * 100) : 0,
      offerConversionRate: submittedCount > 0 ? Math.round((offers / submittedCount) * 100) : 0,
      responseRate: submittedCount > 0 ? Math.round((respondedApps / submittedCount) * 100) : 0,
      vacancyLinkageRate: total > 0 ? Math.round((linkedVacancies / total) * 100) : 0,
    };
  }, [filteredApplications]);

  // Funnel Stages Calculation
  const funnelStages = useMemo<FunnelStageData[]>(() => {
    const submitted = kpiData.submittedApplications || 1;

    const counts: Record<ApplicationStatus, number> = {
      Draft: filteredApplications.filter((a) => a.status === 'Draft').length,
      Applied: filteredApplications.filter((a) => a.status === 'Applied').length,
      Viewed: filteredApplications.filter((a) => a.status === 'Viewed').length,
      HRInterview: filteredApplications.filter((a) => a.status === 'HRInterview').length,
      TechnicalInterview: filteredApplications.filter((a) => a.status === 'TechnicalInterview').length,
      Offer: filteredApplications.filter((a) => a.status === 'Offer').length,
      Rejected: filteredApplications.filter((a) => a.status === 'Rejected').length,
      NoResponse: filteredApplications.filter((a) => a.status === 'NoResponse').length,
    };

    // Cumulative progression count per funnel stage
    const appliedCum = counts.Applied + counts.Viewed + counts.HRInterview + counts.TechnicalInterview + counts.Offer + counts.Rejected + counts.NoResponse;
    const viewedCum = counts.Viewed + counts.HRInterview + counts.TechnicalInterview + counts.Offer;
    const hrCum = counts.HRInterview + counts.TechnicalInterview + counts.Offer;
    const techCum = counts.TechnicalInterview + counts.Offer;
    const offerCum = counts.Offer;

    const stages: { status: ApplicationStatus; label: string; count: number; prevCount: number; color: string }[] = [
      { status: 'Applied', label: '1. Applied (Submitted)', count: appliedCum, prevCount: kpiData.submittedApplications, color: 'bg-blue-500' },
      { status: 'Viewed', label: '2. Viewed by Employer', count: viewedCum, prevCount: appliedCum, color: 'bg-sky-500' },
      { status: 'HRInterview', label: '3. HR Interview', count: hrCum, prevCount: viewedCum, color: 'bg-indigo-500' },
      { status: 'TechnicalInterview', label: '4. Technical Interview', count: techCum, prevCount: hrCum, color: 'bg-purple-500' },
      { status: 'Offer', label: '5. Total Offer', count: offerCum, prevCount: techCum, color: 'bg-emerald-500' },
    ];

    return stages.map((s) => ({
      status: s.status,
      label: s.label,
      count: s.count,
      percentageOfSubmitted: Math.round((s.count / submitted) * 100),
      conversionFromPrevious: s.prevCount > 0 ? Math.round((s.count / s.prevCount) * 100) : 0,
      color: s.color,
    }));
  }, [filteredApplications, kpiData.submittedApplications]);

  // Job Source Performance Comparison
  const jobSourceMetrics = useMemo<JobSourceMetric[]>(() => {
    const map = new Map<
      string,
      {
        total: number;
        submitted: number;
        active: number;
        responded: number;
        noResponse: number;
        hrInterviews: number;
        techInterviews: number;
        offers: number;
        rejected: number;
      }
    >();

    filteredApplications.forEach((a) => {
      const src = a.jobSource || 'Direct / Unknown';
      const curr = map.get(src) || {
        total: 0,
        submitted: 0,
        active: 0,
        responded: 0,
        noResponse: 0,
        hrInterviews: 0,
        techInterviews: 0,
        offers: 0,
        rejected: 0,
      };
      curr.total += 1;
      if (a.status !== 'Draft') {
        curr.submitted += 1;
      }
      if (['Applied', 'Viewed', 'HRInterview', 'TechnicalInterview'].includes(a.status)) {
        curr.active += 1;
      }
      if (['Viewed', 'HRInterview', 'TechnicalInterview', 'Offer', 'Rejected'].includes(a.status)) {
        curr.responded += 1;
      }
      if (a.status === 'NoResponse') {
        curr.noResponse += 1;
      }
      if (['HRInterview', 'TechnicalInterview', 'Offer'].includes(a.status)) {
        curr.hrInterviews += 1;
      }
      if (['TechnicalInterview', 'Offer'].includes(a.status)) {
        curr.techInterviews += 1;
      }
      if (a.status === 'Offer') {
        curr.offers += 1;
      }
      if (a.status === 'Rejected') {
        curr.rejected += 1;
      }
      map.set(src, curr);
    });

    const totalApps = filteredApplications.length || 1;
    const result: JobSourceMetric[] = [];

    map.forEach((data, sourceName) => {
      const sub = data.submitted;
      result.push({
        sourceName,
        totalApplications: data.total,
        submittedCount: sub,
        activeCount: data.active,
        respondedCount: data.responded,
        responseRate: sub > 0 ? Math.round((data.responded / sub) * 100) : 0,
        noResponseCount: data.noResponse,
        noResponseRate: sub > 0 ? Math.round((data.noResponse / sub) * 100) : 0,
        hrInterviewCount: data.hrInterviews,
        hrInterviewRate: sub > 0 ? Math.round((data.hrInterviews / sub) * 100) : 0,
        techInterviewCount: data.techInterviews,
        techInterviewRate: sub > 0 ? Math.round((data.techInterviews / sub) * 100) : 0,
        offerCount: data.offers,
        offerRate: sub > 0 ? Math.round((data.offers / sub) * 100) : 0,
        rejectedCount: data.rejected,
        rejectionRate: sub > 0 ? Math.round((data.rejected / sub) * 100) : 0,
        shareOfTotal: Math.round((data.total / totalApps) * 100),
      });
    });

    return result.sort((a, b) => b.totalApplications - a.totalApplications);
  }, [filteredApplications]);

  // Resume Revision Performance Comparison (ADR 005)
  const resumeRevisionMetrics = useMemo<ResumeRevisionMetric[]>(() => {
    const appsMap = new Map<
      string,
      {
        total: number;
        submitted: number;
        responded: number;
        noResponse: number;
        hrInterviews: number;
        techInterviews: number;
        offers: number;
        rejected: number;
      }
    >();

    filteredApplications.forEach((a) => {
      const revId = a.resumeRevisionId;
      const curr = appsMap.get(revId) || {
        total: 0,
        submitted: 0,
        responded: 0,
        noResponse: 0,
        hrInterviews: 0,
        techInterviews: 0,
        offers: 0,
        rejected: 0,
      };
      curr.total += 1;
      if (a.status !== 'Draft') {
        curr.submitted += 1;
      }
      if (['Viewed', 'HRInterview', 'TechnicalInterview', 'Offer', 'Rejected'].includes(a.status)) {
        curr.responded += 1;
      }
      if (a.status === 'NoResponse') {
        curr.noResponse += 1;
      }
      if (['HRInterview', 'TechnicalInterview', 'Offer'].includes(a.status)) {
        curr.hrInterviews += 1;
      }
      if (['TechnicalInterview', 'Offer'].includes(a.status)) {
        curr.techInterviews += 1;
      }
      if (a.status === 'Offer') {
        curr.offers += 1;
      }
      if (a.status === 'Rejected') {
        curr.rejected += 1;
      }
      appsMap.set(revId, curr);
    });

    return resumeRevisions
      .map((rev) => {
        const stats = appsMap.get(rev.id) || {
          total: 0,
          submitted: 0,
          responded: 0,
          noResponse: 0,
          hrInterviews: 0,
          techInterviews: 0,
          offers: 0,
          rejected: 0,
        };
        const sub = stats.submitted;
        return {
          revisionId: rev.id,
          version: rev.version,
          status: rev.status,
          fullName: rev.personalInfo?.fullName || 'Candidate',
          skillsCount: rev.skills?.length || 0,
          applicationsCount: stats.total,
          submittedCount: sub,
          respondedCount: stats.responded,
          responseRate: sub > 0 ? Math.round((stats.responded / sub) * 100) : 0,
          noResponseCount: stats.noResponse,
          noResponseRate: sub > 0 ? Math.round((stats.noResponse / sub) * 100) : 0,
          hrInterviewCount: stats.hrInterviews,
          hrInterviewRate: sub > 0 ? Math.round((stats.hrInterviews / sub) * 100) : 0,
          techInterviewCount: stats.techInterviews,
          techInterviewRate: sub > 0 ? Math.round((stats.techInterviews / sub) * 100) : 0,
          offersCount: stats.offers,
          offerRate: sub > 0 ? Math.round((stats.offers / sub) * 100) : 0,
          rejectionsCount: stats.rejected,
          rejectionRate: sub > 0 ? Math.round((stats.rejected / sub) * 100) : 0,
        };
      })
      .sort((a, b) => b.version - a.version);
  }, [filteredApplications, resumeRevisions]);

  // Skill Category & Coverage Metrics with Funnel Outcome Correlation
  const skillCategoryMetrics = useMemo<SkillCategoryMetric[]>(() => {
    const categoryLabels: Record<SkillCategory, string> = {
      ProgrammingLanguage: 'Programming Languages',
      Framework: 'Frameworks & Libraries',
      ORM: 'ORM & Data Access',
      Database: 'Databases & Storage',
      Cloud: 'Cloud & Infrastructure',
      DevOps: 'DevOps & CI/CD',
      Messaging: 'Messaging & Queues',
      Testing: 'Testing & QA',
      Tools: 'Development Tools',
      SoftSkill: 'Soft Skills',
      Other: 'Other Technologies',
    };

    // Pre-calculate per-revision application statistics for skill correlation
    const revisionStatsMap = new Map<
      string,
      { submitted: number; hr: number; tech: number; offer: number }
    >();
    filteredApplications.forEach((app) => {
      if (app.status === 'Draft') return;
      const stats = revisionStatsMap.get(app.resumeRevisionId) || {
        submitted: 0,
        hr: 0,
        tech: 0,
        offer: 0,
      };
      stats.submitted += 1;
      if (['HRInterview', 'TechnicalInterview', 'Offer'].includes(app.status)) {
        stats.hr += 1;
      }
      if (['TechnicalInterview', 'Offer'].includes(app.status)) {
        stats.tech += 1;
      }
      if (app.status === 'Offer') {
        stats.offer += 1;
      }
      revisionStatsMap.set(app.resumeRevisionId, stats);
    });

    const categoryMap = new Map<
      SkillCategory,
      Map<
        string,
        {
          name: string;
          totalProficiency: number;
          count: number;
          submittedAppCount: number;
          interviewAppCount: number;
          offerAppCount: number;
        }
      >
    >();

    resumeRevisions.forEach((rev) => {
      const revStats = revisionStatsMap.get(rev.id) || {
        submitted: 0,
        hr: 0,
        tech: 0,
        offer: 0,
      };
      (rev.skills || []).forEach((s) => {
        const cat = s.category || 'Other';
        if (!categoryMap.has(cat)) {
          categoryMap.set(cat, new Map());
        }
        const innerMap = categoryMap.get(cat)!;
        const skillName = s.skillName;
        const existing = innerMap.get(skillName) || {
          name: skillName,
          totalProficiency: 0,
          count: 0,
          submittedAppCount: 0,
          interviewAppCount: 0,
          offerAppCount: 0,
        };
        existing.totalProficiency += s.proficiencyLevel;
        existing.count += 1;
        existing.submittedAppCount += revStats.submitted;
        existing.interviewAppCount += revStats.hr;
        existing.offerAppCount += revStats.offer;
        innerMap.set(skillName, existing);
      });
    });

    const result: SkillCategoryMetric[] = [];

    categoryMap.forEach((skillsMap, category) => {
      const skillsList: SkillItemMetric[] = [];
      let catProfSum = 0;
      let catCount = 0;

      skillsMap.forEach((s) => {
        const avgProf = Math.round((s.totalProficiency / s.count) * 10) / 10;
        skillsList.push({
          name: s.name,
          proficiency: avgProf,
          count: s.count,
          submittedAppCount: s.submittedAppCount,
          interviewAppCount: s.interviewAppCount,
          offerAppCount: s.offerAppCount,
        });
        catProfSum += avgProf;
        catCount += 1;
      });

      result.push({
        category,
        categoryLabel: categoryLabels[category] || category,
        skillCount: skillsList.length,
        averageProficiency: catCount > 0 ? Math.round((catProfSum / catCount) * 10) / 10 : 0,
        skills: skillsList.sort((a, b) => b.proficiency - a.proficiency),
      });
    });

    return result.sort((a, b) => b.skillCount - a.skillCount);
  }, [filteredApplications, resumeRevisions]);

  // Overall Top In-Demand Skills across revisions
  const topSkills = useMemo(() => {
    const allSkills: SkillItemMetric[] = [];
    skillCategoryMetrics.forEach((cat) => {
      cat.skills.forEach((s) => allSkills.push(s));
    });
    return allSkills.sort((a, b) => b.count - a.count || b.proficiency - a.proficiency).slice(0, 10);
  }, [skillCategoryMetrics]);

  // Company Analytics
  const companyMetrics = useMemo<CompanyMetric[]>(() => {
    const compAppMap = new Map<string, { count: number; latestStatus: ApplicationStatus }>();

    filteredApplications.forEach((a) => {
      const curr = compAppMap.get(a.companyId) || { count: 0, latestStatus: a.status };
      curr.count += 1;
      curr.latestStatus = a.status;
      compAppMap.set(a.companyId, curr);
    });

    return companies
      .map((c) => {
        const appData = compAppMap.get(c.id);
        // Only include companies that have at least one application in the filtered set
        if (!appData) return null;
        const hasVacancies = vacancies.some((v) => v.companyId === c.id);
        return {
          companyId: c.id,
          companyName: c.name,
          applicationsCount: appData.count,
          latestStatus: appData.latestStatus,
          hasActiveVacancy: hasVacancies,
        };
      })
      .filter((c): c is CompanyMetric => c !== null)
      .sort((a, b) => b.applicationsCount - a.applicationsCount);
  }, [companies, filteredApplications, vacancies]);

  // Application Relationships & Entity Linkage Summary
  const relationshipMetrics = useMemo(() => {
    const total = filteredApplications.length;
    const vacancyLinked = filteredApplications.filter((a) => Boolean(a.vacancyId)).length;
    const general = total - vacancyLinked;

    const uniqueCompIds = new Set(filteredApplications.map((a) => a.companyId));
    const uniqueVacIds = new Set(
      filteredApplications.map((a) => a.vacancyId).filter((id): id is string => Boolean(id))
    );
    const uniqueRevIds = new Set(filteredApplications.map((a) => a.resumeRevisionId));

    return {
      totalApplications: total,
      vacancyLinkedCount: vacancyLinked,
      generalCount: general,
      uniqueCompaniesCount: uniqueCompIds.size,
      uniqueVacanciesCount: uniqueVacIds.size,
      revisionsUsedCount: uniqueRevIds.size,
    };
  }, [filteredApplications]);

  return {
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
    topSkills,
    companyMetrics,
    relationshipMetrics,
    resumeRevisions,
  };
}
