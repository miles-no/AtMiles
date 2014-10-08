package no.miles.atmiles.employee;

import android.content.Context;
import android.graphics.Bitmap;
import android.os.Looper;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.Collections;
import java.util.List;

import no.miles.atmiles.BuildConfig;
import no.miles.atmiles.EmployeeDetailFragment;
import no.miles.atmiles.R;

public class EmployeeAdapter extends BaseAdapter {
    private List<SearchResultModel.Result> employees = Collections.emptyList();

    private final Context context;

    public EmployeeAdapter(Context context) {
        this.context = context;
    }

    public void updateEmployees(List<SearchResultModel.Result> employees){
        ThreadPreconditions.checkOnMainThread();
        this.employees = employees;
        notifyDataSetChanged();
    }

    public void emptyEmployees(){
        ThreadPreconditions.checkOnMainThread();
        this.employees = Collections.emptyList();
        notifyDataSetChanged();
    }

    @Override
    public int getCount() {
        return employees.size();
    }

    @Override
    public SearchResultModel.Result getItem(int position) {
        return employees.get(position);
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        View rootView = LayoutInflater.from(context)
                .inflate(R.layout.listitem_search_result, parent, false);

        ImageView thumbnailView = (ImageView) rootView.findViewById(R.id.employee_list_thumbnail);
        TextView nameView = (TextView) rootView.findViewById(R.id.employee_list_name);

        SearchResultModel.Result employee = getItem(position);
        nameView.setText(employee.Name);

        Bitmap decodedByte = EmployeeDetailFragment.ConvertToImage(employee.Thumb);
        thumbnailView.setImageBitmap(decodedByte);
        return rootView;
    }

    public static class ThreadPreconditions {
        public static void checkOnMainThread() {
            if (BuildConfig.DEBUG) {
                if (Thread.currentThread() != Looper.getMainLooper().getThread()) {
                    throw new IllegalStateException("This method should be called from the Main Thread");
                }
            }
        }
    }
}
