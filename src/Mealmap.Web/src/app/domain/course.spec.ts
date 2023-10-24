import { Course } from './course';

describe('Class Course', () => {
  const course = new Course(1, 'dummy', 2);

  it('should sort by index ascending', () => {
    const subsequentCourse = new Course(2, 'dummy', 2);
    const result = Course.sort(course, subsequentCourse);
    expect(result).toBeLessThan(0);
  });

  it('should clone to a distinct object', () => {
    const clone = course.clone();
    expect(clone).not.toBe(course);
  });

  it('should clone to an equal object', () => {
    const clone = course.clone();
    expect(clone).toEqual(course);
  });
});
