import React from 'react';
import type { PersonalInfo } from '@/types';
import { User, Mail, Phone, MapPin, Linkedin, Github } from 'lucide-react';

interface PersonalInfoSectionProps {
  personalInfo: PersonalInfo;
  onChange: (info: PersonalInfo) => void;
  isReadOnly?: boolean;
}

export const PersonalInfoSection: React.FC<PersonalInfoSectionProps> = ({
  personalInfo,
  onChange,
  isReadOnly = false,
}) => {
  const handleChange = (field: keyof PersonalInfo, value: string) => {
    onChange({
      ...personalInfo,
      [field]: value,
    });
  };

  return (
    <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-6">
      {/* Section Header */}
      <div className="flex items-center gap-3 border-b border-border/40 pb-4">
        <div className="p-2 rounded-xl bg-primary/10 text-primary border border-primary/20">
          <User className="h-5 w-5" />
        </div>
        <div>
          <h2 className="text-base font-semibold text-foreground">Personal Information</h2>
          <p className="text-xs text-muted-foreground">
            Developer contact details and professional profile links attached to this revision.
          </p>
        </div>
      </div>

      {/* Inputs Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
        {/* Full Name */}
        <div>
          <label className="block text-xs font-medium text-muted-foreground mb-1.5 flex items-center justify-between">
            <span className="flex items-center gap-1.5">
              <User className="h-3.5 w-3.5 text-primary" />
              Full Name <span className="text-destructive">*</span>
            </span>
          </label>
          <div className="relative">
            <input
              type="text"
              required
              disabled={isReadOnly}
              value={personalInfo.fullName}
              onChange={(e) => handleChange('fullName', e.target.value)}
              placeholder="e.g. Alex Johnson"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 transition-colors"
            />
          </div>
        </div>

        {/* Email */}
        <div>
          <label className="block text-xs font-medium text-muted-foreground mb-1.5 flex items-center justify-between">
            <span className="flex items-center gap-1.5">
              <Mail className="h-3.5 w-3.5 text-primary" />
              Email Address <span className="text-destructive">*</span>
            </span>
          </label>
          <div className="relative">
            <input
              type="email"
              required
              disabled={isReadOnly}
              value={personalInfo.email}
              onChange={(e) => handleChange('email', e.target.value)}
              placeholder="e.g. alex.johnson@example.com"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 transition-colors"
            />
          </div>
        </div>

        {/* Phone */}
        <div>
          <label className="block text-xs font-medium text-muted-foreground mb-1.5 flex items-center gap-1.5">
            <Phone className="h-3.5 w-3.5 text-primary" />
            Phone Number
          </label>
          <div className="relative">
            <input
              type="tel"
              disabled={isReadOnly}
              value={personalInfo.phone || ''}
              onChange={(e) => handleChange('phone', e.target.value)}
              placeholder="e.g. +1 (555) 234-5678"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 transition-colors"
            />
          </div>
        </div>

        {/* Location */}
        <div>
          <label className="block text-xs font-medium text-muted-foreground mb-1.5 flex items-center gap-1.5">
            <MapPin className="h-3.5 w-3.5 text-primary" />
            Location
          </label>
          <div className="relative">
            <input
              type="text"
              disabled={isReadOnly}
              value={personalInfo.location || ''}
              onChange={(e) => handleChange('location', e.target.value)}
              placeholder="e.g. San Francisco, CA"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 transition-colors"
            />
          </div>
        </div>

        {/* LinkedIn */}
        <div>
          <label className="block text-xs font-medium text-muted-foreground mb-1.5 flex items-center gap-1.5">
            <Linkedin className="h-3.5 w-3.5 text-primary" />
            LinkedIn Profile URL
          </label>
          <div className="relative">
            <input
              type="url"
              disabled={isReadOnly}
              value={personalInfo.linkedIn || ''}
              onChange={(e) => handleChange('linkedIn', e.target.value)}
              placeholder="https://linkedin.com/in/username"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 transition-colors"
            />
          </div>
        </div>

        {/* GitHub */}
        <div>
          <label className="block text-xs font-medium text-muted-foreground mb-1.5 flex items-center gap-1.5">
            <Github className="h-3.5 w-3.5 text-primary" />
            GitHub Profile URL
          </label>
          <div className="relative">
            <input
              type="url"
              disabled={isReadOnly}
              value={personalInfo.gitHub || ''}
              onChange={(e) => handleChange('gitHub', e.target.value)}
              placeholder="https://github.com/username"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 transition-colors"
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default PersonalInfoSection;
